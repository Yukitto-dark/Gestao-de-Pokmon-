namespace GestaoDePokemon.model
{
    public readonly record struct BaseStats(
        int HP,
        int Atk,
        int Def,
        int SpAtk,
        int SpDef,
        int Speed
    )
    {
        public int Total =>
            HP + Atk + Def + SpAtk + SpDef + Speed;
    }
}