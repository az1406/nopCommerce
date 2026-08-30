using System.Collections.Concurrent;
using NRules;
using NRules.Fluent;

namespace Nop.Services.Rules;

public static class NopRuleEngine
{
    private static readonly ConcurrentDictionary<string, ISessionFactory> _factories = new();

    public static ISession StartSession(string tag, params object[] facts)
    {
        var session = _factories.GetOrAdd(tag, CompileRuleSet).CreateSession();
        session.InsertAll(facts.Where(fact => fact != null));
        session.Fire();

        return session;
    }

    private static ISessionFactory CompileRuleSet(string tag)
    {
        var repository = new RuleRepository();
        repository.Load(spec => spec
            .From(typeof(NopRuleEngine).Assembly)
            .Where(rule => rule.IsTagged(tag))
            .To(tag));

        return repository.Compile();
    }
}