-- Total de rotas cadastradas por região
SELECT r.Name, r.Generation, COUNT(rt.Id) AS TotalRotas
FROM Regions r
LEFT JOIN Routes rt ON rt.RegionId = r.Id
GROUP BY r.Id, r.Name, r.Generation
ORDER BY r.Generation;

-- Pokémon distintos e detalhes de encontro por rota
SELECT rg.Name AS Regiao, rt.Number AS Rota,
       COUNT(DISTINCT e.PokemonId) AS TotalPokemon,
       COUNT(e.Id) AS TotalRegistrosDeEncontro
FROM Routes rt
JOIN Regions rg ON rg.Id = rt.RegionId
LEFT JOIN RouteAreas a ON a.RouteId = rt.Id
LEFT JOIN RoutePokemonEncounters e ON e.RouteAreaId = a.Id
GROUP BY rg.Name, rt.Number
ORDER BY rg.Generation, rt.Number;

-- Detalhes: método, nível, chance, versão e condições
SELECT rg.Name AS Regiao, rt.Number AS Rota, a.Name AS Area,
       p.NationalDexNumber AS Dex, p.Name AS Pokemon,
       e.EncounterMethod, e.MinimumLevel, e.MaximumLevel,
       e.ChancePercent, e.GameVersion, e.ConditionsJson
FROM RoutePokemonEncounters e
JOIN RouteAreas a ON a.Id = e.RouteAreaId
JOIN Routes rt ON rt.Id = a.RouteId
JOIN Regions rg ON rg.Id = rt.RegionId
JOIN Pokemons p ON p.Id = e.PokemonId
ORDER BY rg.Generation, rt.Number, p.NationalDexNumber, e.GameVersion;

-- Deve retornar zero linhas quando as chaves estrangeiras estiverem corretas.
PRAGMA foreign_key_check;
