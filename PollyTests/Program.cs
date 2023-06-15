using Polly;
using Polly.RateLimit;
using Polly.Retry;

ClientWithPolicy client = new(); //Dependency injection

Task thread1 =
    Task.Run(async () =>
    {
        while (true)
        {
            try
            {
                Random r = new();
                int random = r.Next();
                if (random % 2 == 0)
                {
                    await client.DoCallWithPolicyAsync(1);
                }

                if (random % 5 == 0)
                {
                    Console.WriteLine($"Thread #1 - Delaying 2 seconds");
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
            }
        }
    });

Task thread2 =
    Task.Run(async () =>
    {
        while (true)
        {
            try
            {
                Random r = new();
                int random = r.Next();
                if (random % 2 != 0)
                {
                    await client.DoCallWithPolicyAsync(2);
                }

                if (random % 5 == 0)
                {
                    Console.WriteLine($"Thread #2 - Delaying 3 seconds");
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
            }
        }
    });

await Task.WhenAll(thread1, thread2);

class ClientWithPolicy
{
    private int _count = 0;

    private readonly AsyncPolicy _rateLimitPolicy = Policy.RateLimitAsync(3, TimeSpan.FromSeconds(5), 3);

    private readonly AsyncRetryPolicy _retryPolicy =
        Policy
            .Handle<RateLimitRejectedException>()
            .WaitAndRetryAsync
            (
                retryCount: 10,
                (retryNumber, context) => (TimeSpan)context["Retry-After"],
                (ex, ts, n, ctx) => Console.WriteLine($"Thread #{ctx["ThreadNumber"]} - Call #{ctx["c"]} - retry attempt #{n}")
            );

    public async Task DoCallWithPolicyAsync(int threadNumber)
    {
        int c = Interlocked.Increment(ref _count);

        await _retryPolicy
            .ExecuteAsync(async context =>
            {
                try
                {
                    AddOverwrite(context, "ThreadNumber", threadNumber);
                    AddOverwrite(context, "c", c);

                    await _rateLimitPolicy.ExecuteAsync(async () => await DoCallAsync(threadNumber, c));
                }
                catch (RateLimitRejectedException ex)
                {
                    //context.TryAdd("Retry-After", ex.RetryAfter);
                    AddOverwrite(context, "Retry-After", ex.RetryAfter);
                    throw;
                }
            },
            new Context());
    }

    private static Task DoCallAsync(int threadNumber, int callNumber)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss:ff} Thread #{threadNumber} - Doing the call #{callNumber}");

        Random r = new();
        int random = r.Next();
        if (random % 2 != 0)
        {
            throw new ArgumentException("Random exception");
        }

        return Task.CompletedTask;
    }

    private static void AddOverwrite<TKey, TValue>(IDictionary<TKey, TValue> context, TKey key, TValue value)
    {
        if (context.ContainsKey(key))
        {
            context[key] = value;
        }
        else
        {
            context.Add(key, value);
        }
    }
}