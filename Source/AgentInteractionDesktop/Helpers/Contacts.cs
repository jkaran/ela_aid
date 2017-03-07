namespace Agent.Interaction.Desktop.Helpers
{
    public class Contacts : IContacts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Contacts"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="number">The number.</param>
        /// <param name="type">The type.</param>
        #region Contacts
        public Contacts(string name, string number, string type)
        {
            Name = name;
            Number = number;
            Type = type;
        } 
        #endregion

        #region Properties
        public string Name { get; set; }

        public string Number { get; set; }

        public string Type { get; set; } 
        #endregion
    }
}