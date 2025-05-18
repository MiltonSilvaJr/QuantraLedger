
using Sprache;
using Quantra.Domain.Models;

namespace Quantra.Transaction;

public static class TxDslParser
{
    private static readonly Parser<string> Identifier =
        Parse.Letter.Once().Concat(Parse.LetterOrDigit.Many()).Text();

    private static readonly Parser<string> Direction =
        Parse.String("debit").Text().Or(Parse.String("credit").Text());

    private static readonly Parser<decimal> Amount =
        Parse.Decimal.Select(decimal.Parse);

    private static readonly Parser<string> Currency =
        Parse.Letter.Repeat(3).Text();

    private static readonly Parser<LedgerInstruction> Instruction =
        from dir in Direction.Token()
        from acc in Identifier.Token()
        from amt in Amount.Token()
        from cur in Currency.Token()
        select new LedgerInstruction(acc, dir, amt, cur);

    private static readonly Parser<IEnumerable<LedgerInstruction>> Script =
        Instruction.DelimitedBy(Parse.Char(';').Token());

    public static IEnumerable<LedgerInstruction> ParseScript(string script) =>
        Script.End().Parse(script);
}
