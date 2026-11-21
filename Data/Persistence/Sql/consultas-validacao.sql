-- Quantidade de espécies e formas cadastradas.
SELECT COUNT(*) AS TotalPokemons FROM Pokemons;
SELECT COUNT(*) AS TotalFormas FROM PokemonForms;

-- Base Stats são persistidos como JSON na coluna Stats de PokemonForms.
SELECT COUNT(*) AS FormasComBaseStats
FROM PokemonForms
WHERE Stats IS NOT NULL AND TRIM(Stats) <> '';

-- Catálogo e relacionamentos de movimentos.
SELECT COUNT(*) AS TotalMovimentos FROM Moves;
SELECT COUNT(*) AS TotalLearnsets FROM LearnableMoves;
SELECT COUNT(DISTINCT f.PokemonId) AS PokemonsComLearnset
FROM LearnableMoves lm
JOIN PokemonForms f ON f.Id = lm.PokemonFormId
WHERE f.FormName = 'Normal';

-- Distribuição por método e grupo de jogos.
SELECT Method, GameVersion, COUNT(*) AS Quantidade
FROM LearnableMoves
GROUP BY Method, GameVersion
ORDER BY GameVersion, Method;

-- Pokémon sem nenhum learnset associado à forma Normal.
SELECT p.NationalDexNumber, p.Name
FROM Pokemons p
JOIN PokemonForms f ON f.PokemonId = p.Id AND f.FormName = 'Normal'
LEFT JOIN LearnableMoves lm ON lm.PokemonFormId = f.Id
GROUP BY p.Id, p.NationalDexNumber, p.Name
HAVING COUNT(lm.Id) = 0
ORDER BY p.NationalDexNumber;


-- Quantidade de relações diretas de evolução e pares origem/destino distintos.
SELECT COUNT(*) AS TotalRegistrosEvolucao FROM Evolutions;

SELECT COUNT(DISTINCT FromPokemonId || '-' || ToPokemonId) AS ParesEvolutivosDistintos
FROM Evolutions;

-- Cadeias: Pokémon de origem, Pokémon de destino e requisitos cadastrados.
SELECT
    origem.NationalDexNumber AS DexOrigem,
    origem.Name AS PokemonOrigem,
    destino.NationalDexNumber AS DexDestino,
    destino.Name AS PokemonDestino,
    e.Method,
    e.RequiredLevel,
    e.RequiredItem,
    e.RequiredTime,
    e.MinimumFriendship,
    e.MinimumBeauty,
    e.RequiredGender,
    e.RequiredLocation,
    e.GameVersion
FROM Evolutions e
JOIN Pokemons origem ON origem.Id = e.FromPokemonId
JOIN Pokemons destino ON destino.Id = e.ToPokemonId
ORDER BY origem.NationalDexNumber, destino.NationalDexNumber;

-- Pokémon que possuem pelo menos uma evolução ou pré-evolução registrada.
SELECT COUNT(DISTINCT PokemonId) AS PokemonsEmLinhasEvolutivas
FROM (
    SELECT FromPokemonId AS PokemonId FROM Evolutions
    UNION
    SELECT ToPokemonId AS PokemonId FROM Evolutions
);
