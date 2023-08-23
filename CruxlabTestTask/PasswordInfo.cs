namespace CruxlabTestTask
{
    class PasswordInfo
    {
        public string Password { get; init; } = "";

        public char RequiredSymbol { get; init; }

        public int MinAmount { get; init; }

        public int MaxAmount { get; init; }
    }
}
