using UnityEngine;
using System.Collections;

public class ResultControl {

    // Oni Defeat Rank
    private const int OniDefeatRankExcellent = 400;
    private const int OniDefeatRankGood = 200;
    private const int OniDefeatRankNormal = 100;
    // Evaluation Rank
    private const int EvaluationRankExcellent = 160;
    private const int EvaluationRankGood = 80;
    private const int EvaluationRankNormal = 40;
    // Total Rank
    private const int TotalRankExcellent = 40;
    private const int TotalRankGood = 32;
    private const int TotalRankNormal = 10;
    // Evaluation Score
    private const int EvaluationGreatScore = 4;
    private const int EvaluationGoodScore = 2;
    private const int EvaluationOkayScore = 1;
    private const int EvaluationMissScore = 0;
    // Oni Defeat Rank Point
    private const int OniDefeatRankExcellentPoint = 10;
    private const int OniDefeatRankGoodPoint = 8;
    private const int OniDefeatRankNormalPoint = 5;
    private const int OniDefeatRankBadPoint = 3;
    // Evaluation Rank Point
    private const int EvaluationRankExcellentPoint = 5;
    private const int EvaluationRankGoodPoint = 3;
    private const int EvaluationRankNormalPoint = 2;
    private const int EvaluationRankBadPoint = 1;
    // Total Rank Point
    private const int TotalRankExcellentPoint = 15;
    private const int TotalRankGoodPoint = 11;
    private const int TotalRankNormalPoint = 7;
    private const int TotalRankBadPoint = 0;

    public int oniDefeatScore = 0;
    public int evaluationScore = 0;

    public void AddOniDefeatScore(int defeatNum)
    {
        oniDefeatScore += defeatNum;
    }

    public void AddEvaluationScore(GameSceneControl.Evaluation rank)
    {
        switch (rank)
        {
            case GameSceneControl.Evaluation.Okay:
                evaluationScore += EvaluationOkayScore;
                break;
            case GameSceneControl.Evaluation.Good:
                evaluationScore += EvaluationGoodScore;
                break;
            case GameSceneControl.Evaluation.Great:
                evaluationScore += EvaluationGreatScore;
                break;
            case GameSceneControl.Evaluation.Miss:
                evaluationScore += EvaluationMissScore;
                break;
        }
    }

    public int GetDefeatRank()
    {
        if (oniDefeatScore >= OniDefeatRankExcellent) return 3;
        else if (oniDefeatScore >= OniDefeatRankGood) return 2;
        else if (oniDefeatScore >= OniDefeatRankNormal) return 1;
        else return 0;
    }

    public int GetEvaluationRank()
    {
        if (evaluationScore >=EvaluationRankExcellent) return 3;
        else if (evaluationScore >= EvaluationRankGood) return 2;
        else if (evaluationScore >= EvaluationRankNormal) return 1;
        else return 0;
    }

    public int GetTotalRank()
    {
        int defeatPoint = 0;
        if (oniDefeatScore >= OniDefeatRankExcellent)
            defeatPoint = OniDefeatRankExcellentPoint;
        else if (oniDefeatScore >= OniDefeatRankGood)
            defeatPoint = OniDefeatRankGoodPoint;
        else if (oniDefeatScore >= OniDefeatRankNormal)
            defeatPoint = OniDefeatRankNormalPoint;
        else
            defeatPoint = OniDefeatRankBadPoint;

        int evaluationPoint = 0;
        if (evaluationScore >= EvaluationRankExcellent)
            evaluationPoint = EvaluationRankExcellentPoint;
        else if (evaluationScore >= EvaluationRankGood)
            evaluationPoint = EvaluationRankGoodPoint;
        else if (evaluationScore >= EvaluationRankNormal)
            evaluationPoint = EvaluationRankNormalPoint;
        else
            evaluationPoint = EvaluationRankBadPoint;

        int totalPoint = defeatPoint + evaluationPoint;
        if (totalPoint >= TotalRankExcellentPoint) return 3;
        else if (totalPoint >= TotalRankGoodPoint) return 2;
        else if (totalPoint >= TotalRankNormalPoint) return 1;
        else return 0;
    }
}
