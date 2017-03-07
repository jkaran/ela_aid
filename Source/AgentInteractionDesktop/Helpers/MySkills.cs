namespace Agent.Interaction.Desktop.Helpers
{
    public class MySkills : IMySkills
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySkills"/> class.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        /// <param name="skillLevel">The skill level.</param>
        #region MySkills
        public MySkills(string skillName, int skillLevel)
        {
            SkillName = skillName;
            SkillLevel = skillLevel;
        } 
        #endregion

        #region Properties
        public string SkillName { get; set; }

        public int SkillLevel { get; set; } 
        #endregion
    }
}