using Nop.Core.Domain.Directory;
using NRules.Fluent.Dsl;

namespace Nop.Services.Catalog.Rules;

[Name("A price that is not on a five-cent step is rounded up to the next one"), Tag(CashRounding.Tag)]
public class Rounding005UpRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding005Up,
                r => r.Cents % 5 != 0);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price + (5 - r.Cents % 5) / 100 }));
    }
}

[Name("A price that is not on a five-cent step is rounded down to the previous one"), Tag(CashRounding.Tag)]
public class Rounding005DownRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding005Down,
                r => r.Cents % 5 != 0);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price - r.Cents % 5 / 100 }));
    }
}

[Name("A price whose last cent digit is below five is rounded down to the ten-cent step when rounding to ten cents upward"), Tag(CashRounding.Tag)]
public class Rounding01UpBelowHalfRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding01Up,
                r => r.Cents % 10 < 5);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price - r.Cents % 10 / 100 }));
    }
}

[Name("A price whose last cent digit is five or more is rounded up to the ten-cent step when rounding to ten cents upward"), Tag(CashRounding.Tag)]
public class Rounding01UpFromHalfRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding01Up,
                r => r.Cents % 10 >= 5);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price + (10 - r.Cents % 10) / 100 }));
    }
}

[Name("A price whose last cent digit is five or less is rounded down to the ten-cent step when rounding to ten cents downward"), Tag(CashRounding.Tag)]
public class Rounding01DownToHalfRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding01Down,
                r => r.Cents % 10 <= 5);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price - r.Cents % 10 / 100 }));
    }
}

[Name("A price whose last cent digit is above five is rounded up to the ten-cent step when rounding to ten cents downward"), Tag(CashRounding.Tag)]
public class Rounding01DownAboveHalfRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding01Down,
                r => r.Cents % 10 > 5);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price + (10 - r.Cents % 10) / 100 }));
    }
}

[Name("A price with fewer than twenty-five cents is rounded down to the whole unit when rounding to fifty cents"), Tag(CashRounding.Tag)]
public class Rounding05LowRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding05,
                r => r.Cents < 25);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price - r.Cents / 100 }));
    }
}

[Name("A price with twenty-five to seventy-four cents is rounded to the half unit when rounding to fifty cents"), Tag(CashRounding.Tag)]
public class Rounding05MiddleRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding05,
                r => r.Cents >= 25,
                r => r.Cents < 75);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price + (50 - r.Cents) / 100 }));
    }
}

[Name("A price with seventy-five cents or more is rounded up to the next whole unit when rounding to fifty cents"), Tag(CashRounding.Tag)]
public class Rounding05HighRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding05,
                r => r.Cents >= 75);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = r.Price + (100 - r.Cents) / 100 }));
    }
}

[Name("A price with fewer than fifty cents is rounded down to the whole unit when rounding to whole units"), Tag(CashRounding.Tag)]
public class Rounding1BelowHalfRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding1,
                r => r.Cents < 50);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = Math.Truncate(r.Price) }));
    }
}

[Name("A price with fifty cents or more is rounded up to the next whole unit when rounding to whole units"), Tag(CashRounding.Tag)]
public class Rounding1FromHalfRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding1,
                r => r.Cents >= 50);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = Math.Truncate(r.Price) + 1 }));
    }
}

[Name("A price with any cents is rounded up to the next whole unit when rounding to whole units upward"), Tag(CashRounding.Tag)]
public class Rounding1UpWithCentsRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding1Up,
                r => r.Cents > 0);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = Math.Truncate(r.Price) + 1 }));
    }
}

[Name("A price without cents is kept at its whole unit when rounding to whole units upward"), Tag(CashRounding.Tag)]
public class Rounding1UpWithoutCentsRule : Rule
{
    public override void Define()
    {
        RoundingRequest r = default;

        When()
            .Match(() => r,
                r => r.Type == RoundingType.Rounding1Up,
                r => r.Cents <= 0);

        Then()
            .Do(ctx => ctx.Insert(new RoundedPrice { Value = Math.Truncate(r.Price) }));
    }
}