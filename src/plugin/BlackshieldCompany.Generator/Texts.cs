namespace BlackshieldCompany.Generator;

/// <summary>All player-facing text in one place.</summary>
internal static class Texts
{
    public const string LetterName = "Sealed Letter (Blackshield Company)";
    public const string Letter =
@"<font face='$HandwrittenFont'>
To the one they've been talking about,

Word of your blade has reached the Blackshield Company. We are a free company of sellswords. We take coin for honest killing, we keep our contracts, and we keep each other breathing.

We are hiring. Sign, and your name goes into the company ledger as a Recruit. Contracts pay in gold and in renown, and renown is how a Recruit becomes a Commander.

A share of every purse goes to the company treasury. Officers draw on it. That is how a company outlives its founders.

Hold the line.

- The Quartermaster, Blackshield Company
</font>";

    public const string LedgerName = "Blackshield Company Ledger";
    public const string Ledger =
@"<p align='center'><font size='40'>BLACKSHIELD COMPANY</font></p>
<p align='center'>Ledger and Articles of Contract</p>

[pagebreak]

<p align='center'><font size='28'>The Articles</font></p>

I. A contract taken is a contract kept. A sword who abandons one loses renown.

II. Every purse is shared. Part of the pay for every contract goes to the company treasury.

III. Renown decides rank. Rank decides who leads.

[pagebreak]

<p align='center'><font size='28'>Ranks</font></p>

Recruit: all who sign.

Blade: 100 renown. Issued a Blackshield Blade.

Sergeant: 300 renown. Issued a Blackshield Shield.

Captain: 700 renown. Issued Blackshield Plate. May draw on the treasury and call a company sellsword.

Commander: 1500 renown. Leads the company. Contracts pay more.

[pagebreak]

<p align='center'><font size='28'>Contracts</font></p>

Camp clearing: a bandit camp has a price on its leader.

Bounties: a marked target hides near a ruin or cave.

Holding actions: raiders are moving on a fort and the company wants the ground held.

To take work, use the Company Ledger power or read this ledger.";

    public const string Courier =
"A courier catches up with you on the road.\n\n\"Sealed letter, for you. Already paid for.\"\n\nThey press it into your hand and are gone before you can ask who sent it.";

    public const string JoinOffer =
"Sign the Blackshield Company contract?\n\nYou join as a Recruit. Company contracts pay gold and renown, and a share of every purse goes to the company treasury.";

    public const string Welcome =
"Your name is entered in the ledger. You are a Recruit of the Blackshield Company.\n\nYou have received the Company Ledger and the Company Ledger power. Use either one to take contracts.";

    public const string NotMember =
"This ledger belongs to the Blackshield Company. You have not signed its contract.";

    public static readonly string[] RankNames = { "Recruit", "Blade", "Sergeant", "Captain", "Commander" };

    public static string LedgerMenu(int rank) => rank < 4
        ? $"BLACKSHIELD COMPANY LEDGER\n\nRank: {RankNames[rank]}\nRenown: %.0f (next rank at %.0f)\nTreasury: %.0f gold\nContracts fulfilled: %.0f"
        : "BLACKSHIELD COMPANY LEDGER\n\nRank: Commander\nRenown: %.0f\nTreasury: %.0f gold\nContracts fulfilled: %.0f";

    public const string ContractMenu =
"Choose a contract.\n\nPay rises with your rank and experience. A share of every purse goes to the company treasury.";

    public const string NoContract =
"No contract of that kind is posted right now. Try another kind of work, or check back later.";

    public const string ContractStatus =
"You are already under contract. Your journal has the details.";

    public const string ContractPaid =
"Contract fulfilled.\n\nYour pay: %.0f gold\nTo the company treasury: %.0f gold\nRenown: +%.0f (now %.0f)";

    public const string Treasury = "The company treasury holds %.0f gold.";

    public const string TreasuryLocked =
"The company treasury holds %.0f gold.\n\nOnly a Captain or higher may draw on it.";

    public static readonly string[] Promotions =
    {
        "",
        "Promoted to Blade.\n\nThe quartermaster sends you a Blackshield Blade.",
        "Promoted to Sergeant.\n\nYou are issued a Blackshield Shield.",
        "Promoted to Captain.\n\nYou are issued Blackshield Plate. You may now draw on the company treasury and call a company sellsword from the ledger.",
        "Promoted to Commander.\n\nThe Blackshield Company is yours to lead. Your contracts pay a quarter more, and you gain Commander's Presence.",
    };
}
