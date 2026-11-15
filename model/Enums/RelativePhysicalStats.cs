namespace GestaoDePokemon.model
{
    /// <summary>
    /// Relação entre Attack e Defense exigida por algumas evoluções.
    /// Valores seguem a convenção da PokéAPI: -1, 0 e 1.
    /// </summary>
    public enum RelativePhysicalStats
    {
        AttackLowerThanDefense = -1,
        AttackEqualsDefense = 0,
        AttackHigherThanDefense = 1
    }
}
