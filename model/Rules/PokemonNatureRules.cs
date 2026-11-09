namespace GestaoDePokemon.model
{
    public static class PokemonNatureRules
    {
        public static NatureModifiers GetModifiers(
            PokemonNature nature)
        {
            return nature switch
            {
                PokemonNature.Lonely =>
                    new(PokemonStat.Attack, PokemonStat.Defense),

                PokemonNature.Brave =>
                    new(PokemonStat.Attack, PokemonStat.Speed),

                PokemonNature.Adamant =>
                    new(PokemonStat.Attack, PokemonStat.SpecialAttack),

                PokemonNature.Naughty =>
                    new(PokemonStat.Attack, PokemonStat.SpecialDefense),

                PokemonNature.Bold =>
                    new(PokemonStat.Defense, PokemonStat.Attack),

                PokemonNature.Relaxed =>
                    new(PokemonStat.Defense, PokemonStat.Speed),

                PokemonNature.Impish =>
                    new(PokemonStat.Defense, PokemonStat.SpecialAttack),

                PokemonNature.Lax =>
                    new(PokemonStat.Defense, PokemonStat.SpecialDefense),

                PokemonNature.Timid =>
                    new(PokemonStat.Speed, PokemonStat.Attack),

                PokemonNature.Hasty =>
                    new(PokemonStat.Speed, PokemonStat.Defense),

                PokemonNature.Jolly =>
                    new(PokemonStat.Speed, PokemonStat.SpecialAttack),

                PokemonNature.Naive =>
                    new(PokemonStat.Speed, PokemonStat.SpecialDefense),

                PokemonNature.Modest =>
                    new(PokemonStat.SpecialAttack, PokemonStat.Attack),

                PokemonNature.Mild =>
                    new(PokemonStat.SpecialAttack, PokemonStat.Defense),

                PokemonNature.Quiet =>
                    new(PokemonStat.SpecialAttack, PokemonStat.Speed),

                PokemonNature.Rash =>
                    new(PokemonStat.SpecialAttack, PokemonStat.SpecialDefense),

                PokemonNature.Calm =>
                    new(PokemonStat.SpecialDefense, PokemonStat.Attack),

                PokemonNature.Gentle =>
                    new(PokemonStat.SpecialDefense, PokemonStat.Defense),

                PokemonNature.Sassy =>
                    new(PokemonStat.SpecialDefense, PokemonStat.Speed),

                PokemonNature.Careful =>
                    new(PokemonStat.SpecialDefense, PokemonStat.SpecialAttack),

                PokemonNature.Hardy or
                PokemonNature.Docile or
                PokemonNature.Serious or
                PokemonNature.Bashful or
                PokemonNature.Quirky =>
                    new(null, null),

                _ => throw new ArgumentOutOfRangeException(nameof(nature))
            };
        }
    }
}