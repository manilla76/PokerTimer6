namespace SheepheadLibrary;

public static class PointService
{
    public static int AddPoints(List<Player> team, List<Trick> tricks)
    {
        int score = 0;
        foreach (Player player in team)   // go through the tricks won by each player on the team
        {
            foreach (var trick in player.TricksWon)
            {
                score += tricks[trick].TrickCards.Sum(c => c.Points);  // go through each trick and add all the points to the score
            }
        }
        return score;
    }

    public static int CalculateWinner(int queenScore, int nonQueenScore, bool acrossTable = false)
    {

        // TODO: fix solo scoring
        if (queenScore == nonQueenScore && acrossTable) 
        {
            return 0;   // tie
        }
        if (queenScore == 120)  // like 12 O'clock but not fully corrent.  TODO: Fix 12 O'Clock
        {
            if (!acrossTable)
                return 4;
            else return 8;
        }
        if (queenScore > 90)
        {
            if (!acrossTable)
                return 2;
            else return 4;
        }
        if (queenScore > 60)
        {
            if (!acrossTable)
                return 1;
            else return 2;
        }
        if (nonQueenScore == 120)
        {
            return 8;    // across table 12 O'Clock
        }
        if (nonQueenScore >= 90)
        {
            if (!acrossTable)
                return 3;
            else return 4;
        }
        if (!acrossTable)
            return 2;
        else return 2;
    }
}
