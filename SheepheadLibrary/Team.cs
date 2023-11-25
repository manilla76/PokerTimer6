namespace SheepheadLibrary;

public class Team
{
    public List<Player> TeamQueen { get; private set; } = new List<Player>();
    public List<Player> TeamNonQueen { get; private set; } = new List<Player>();
    public bool NeedsFirstTrick { get; private set; } = false;
    /// <summary>
    /// Set the 2 teams if possible, if not possible pass that back to caller
    /// </summary>
    /// <param name="players"></param>
    /// <param name="dealer"></param>
    /// <param name="isAcrossTable"></param>
    /// <returns>True teams fully set.  Otherwise false.</returns>
    public bool SetTeams(List<Player> players, int dealer, bool isAcrossTable = false)
    {
        TeamQueen.Clear();
        TeamNonQueen.Clear();

        if (isAcrossTable)
        {
            SetupAcrossTheTable(players, dealer);
            return true;
        }

        return (FindQueensInPlayerHands(players)); // true if both queens in the same hand, otherwise false
    }
    /// <summary>
    /// Set Teams when given the team members from outside (first trick or solo)
    /// </summary>
    /// <param name="players"></param>
    /// <param name="playerA"></param>
    /// <param name="playerB"></param>
    public void SetTeams(List<Player> players, Player playerA, Player? playerB = null)
    {
        NeedsFirstTrick = false;
        AssignTeamQueen(players, playerA, playerB);
    }

    /// <summary>
    /// Checks all player hands to see who has black queens.
    /// </summary>
    /// <returns>True if Teams fully set</returns>
    /// <exception></exception>
    private bool FindQueensInPlayerHands(List<Player> players)
    {
        Player playerWithQC = players.First(c => c.Hand.Cards.Any(i => (i.Suit == CardSuit.Club && i.Weight == CardWeight.Queen)));  // find player with QC
        Player playerWithQS = players.First(c => c.Hand.Cards.Any(i => (i.Suit == CardSuit.Spade && i.Weight == CardWeight.Queen))); // find player with QS
        if (playerWithQS != playerWithQC)
        {
            AssignTeamQueen(players, playerWithQC, playerWithQS);
            return true;
        }
        NeedsFirstTrick = true;
        playerWithQC.HasBothQueens = true;  // set flag on player with both black Queens
        return false;
    }

    private void AssignTeamQueen(List<Player> players, Player playerA, Player? playerB = null)
    {
        TeamQueen.Add(playerA);
        if (playerB is not null)   // null for solo
        {
            TeamQueen.Add(playerB);  
        }
        TeamNonQueen = players.Where(p => !TeamQueen.Contains(p)).ToList();
    }

    private void SetupAcrossTheTable(List<Player> players, int dealer)
    {
        List<Player> playersNotAlreadyOut = players.Where(p=>p.IsOut == false).ToList();
        int dealerOffset = 2; // across the table
        if (Math.Abs(playersNotAlreadyOut[0].Number % 4 - playersNotAlreadyOut[1].Number % 4) == 2)
        {
            dealerOffset = 1; // ahead of the deal
        }
        TeamQueen.Add(players[dealer]);
        TeamQueen.Add(players[(dealer + dealerOffset) % 4]);
        TeamNonQueen = players.Where(p => ! TeamQueen.Contains(p)).ToList();
    }
}
