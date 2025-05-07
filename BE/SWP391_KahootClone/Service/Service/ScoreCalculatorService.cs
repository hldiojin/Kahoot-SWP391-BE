using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class ScoreCalculatorService
    {
        /// <summary>
        /// Calculates the score for a player in solo mode.
        /// </summary>
        /// <param name="playerAnswer">The player's answer to a question.</param>
        /// <param name="question">The question that was answered.</param>
        /// <returns>The score for the player for that question.</returns>
        public int CalculateSoloScore(PlayerAnswerDTO playerAnswer, QuestionDTO question)
        {
            // Validate input
            if (playerAnswer == null)
            {
                throw new ArgumentNullException(nameof(playerAnswer), "Player answer cannot be null.");
            }
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question), "Question cannot be null.");
            }

            // Initialize score
            int score = 0;

            // Check if the answer is correct
            if (playerAnswer.IsCorrect)
            {
                score = question.Score; // Award the question's score if correct.

                // Consider time limit and response time.
                if (question.TimeLimit.HasValue)
                {
                    // Calculate a bonus based on how quickly the player answered.
                    int timeBonus = CalculateTimeBonus(playerAnswer.ResponseTime, question.TimeLimit.Value);
                    score += timeBonus;
                }
            }

            return score;
        }

        /// <summary>
        /// Calculates a bonus score based on the player's response time.
        /// </summary>
        /// <param name="responseTime">The time taken by the player to answer (in seconds).</param>
        /// <param name="timeLimit">The time limit for the question (in seconds).</param>
        /// <returns>The bonus score.</returns>
        private int CalculateTimeBonus(int responseTime, int timeLimit)
        {
            if (responseTime <= 0)
                return 0;

            if (responseTime >= timeLimit)
                return 0;

            // Linear bonus calculation (can be adjusted)
            // The faster the response, the higher the bonus.
            double timeRatio = (double)responseTime / timeLimit;
            int bonus = (int)((1 - timeRatio) * 20); // Example: Max bonus of 20, decreasing linearly.

            return Math.Max(0, bonus); // Ensure bonus is not negative.
        }

        /// <summary>
        /// Calculates the score for a group based on the individual player answers.
        /// </summary>
        /// <param name="groupMembers">The members of the group.</param>
        /// <param name="playerAnswers">The answers given by the players in the group.</param>
        /// <param name="questions">The questions that were answered.</param>
        /// <returns>A dictionary of player scores, and the total group score.</returns>
        public (Dictionary<int, int> playerScores, int totalGroupScore) CalculateGroupScore(
            List<GroupMemberDTO> groupMembers,
            List<PlayerAnswerDTO> playerAnswers,
            List<QuestionDTO> questions)
        {
            // Validate input
            if (groupMembers == null)
            {
                throw new ArgumentNullException(nameof(groupMembers), "Group members cannot be null.");
            }
            if (playerAnswers == null)
            {
                throw new ArgumentNullException(nameof(playerAnswers), "Player answers cannot be null.");
            }
            if (questions == null)
            {
                throw new ArgumentNullException(nameof(questions), "Questions cannot be null.");
            }

            // Initialize the dictionary to store each player's score.
            Dictionary<int, int> playerScores = new Dictionary<int, int>();
            int totalGroupScore = 0;

            // Populate the playerScores dictionary with initial scores of 0 for each member.
            foreach (var member in groupMembers)
            {
                playerScores[member.PlayerId] = 0;
            }

            // Iterate through each player's answer to calculate scores.
            foreach (var playerAnswer in playerAnswers)
            {
                // Find the corresponding question.
                QuestionDTO question = questions.FirstOrDefault(q => q.Id == playerAnswer.QuestionId);
                if (question != null)
                {
                    // Calculate the score for the player for this question.
                    int playerScore = CalculateSoloScore(playerAnswer, question); // Reuse the solo score calculation

                    // Add the score to the player's total.
                    playerScores[playerAnswer.PlayerId] += playerScore;
                    totalGroupScore += playerScore; // Accumulate the total group score.
                }
                // else:  Question not found.  This might indicate an error, but we'll just skip it.
            }
            return (playerScores, totalGroupScore);
        }
    }
}
