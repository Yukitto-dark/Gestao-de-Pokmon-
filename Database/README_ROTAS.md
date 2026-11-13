# Regiões, rotas e encontros — SistemaDeGestaoDePokemon

Este pacote adiciona ao SQLite os dados de Kanto, Johto e Hoenn e apenas as
rotas numeradas. Cada rota é dividida em suas áreas da PokéAPI; cada encontro
armazena espécie, método, níveis mínimo/máximo, chance, versão do jogo e
condições do encontro.

## Escopo

- Kanto: rotas 1–28.
- Johto: rotas 29–46.
- Hoenn: rotas 101–134.
- Versões consultadas: Red, Blue, Yellow, Gold, Silver, Crystal, FireRed,
  LeafGreen, Ruby, Sapphire e Emerald (filtradas pela região).
- Somente espécies existentes na tabela `Pokemons` do banco local são
  relacionadas aos encontros.

## Estrutura criada/atualizada

- `Regions`: mantém `Id`, `Name` e `Generation`; acrescenta `Description` e
  `MainGames` se ainda não existirem.
- `Routes`: número, região, slug e identificador da localização na PokéAPI,
  além das versões com dados de encontro.
- `RouteAreas`: subáreas retornadas para cada rota.
- `RoutePokemonEncounters`: relação entre a área e o Pokémon, incluindo método,
  faixa de nível, chance, versão e condições.

A importação é idempotente. Ao repetir a execução, os encontros de cada rota
importada são atualizados, sem duplicar regiões, rotas ou encontros. Antes de
alterar o banco, o script cria uma cópia de segurança automaticamente.

## Execução

Na raiz do projeto, execute:

```bash
python3 scripts/importar_rotas_regioes.py \
  --database "$HOME/.wine-dotnet-10/drive_c/users/jose/AppData/Local/GestaoDePokemon/gestaopokemon.db"
```

A primeira execução precisa de internet para consultar a PokéAPI. Os JSONs
recebidos ficam em `~/.cache/gestaopokemon/pokeapi-rotas` e são reutilizados nas
execuções seguintes. Para atualizar os dados da fonte:

```bash
python3 scripts/importar_rotas_regioes.py \
  --database "$HOME/.wine-dotnet-10/drive_c/users/jose/AppData/Local/GestaoDePokemon/gestaopokemon.db" \
  --refresh-api
```

Se seu arquivo SQLite estiver em outro local, substitua o caminho passado em
`--database`. O banco precisa conter `Pokemons(Id, NationalDexNumber)` e,
quando já houver `Regions`, ela deve conter `Id`, `Name` e `Generation`.

## Consultas de conferência

```sql
SELECT r.Name, r.Generation, COUNT(rt.Id) AS TotalRotas
FROM Regions r
LEFT JOIN Routes rt ON rt.RegionId = r.Id
GROUP BY r.Id, r.Name, r.Generation
ORDER BY r.Generation;
```

```sql
SELECT rg.Name AS Regiao, rt.Number AS Rota,
       COUNT(DISTINCT e.PokemonId) AS TotalPokemon,
       COUNT(e.Id) AS TotalRegistrosDeEncontro
FROM Routes rt
JOIN Regions rg ON rg.Id = rt.RegionId
LEFT JOIN RouteAreas a ON a.RouteId = rt.Id
LEFT JOIN RoutePokemonEncounters e ON e.RouteAreaId = a.Id
GROUP BY rg.Name, rt.Number
ORDER BY rg.Generation, rt.Number;
```

```sql
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
```

```sql
PRAGMA foreign_key_check;
```

## Observação sobre a tela de regiões

No código que acompanha o projeto, `RegioesController` ainda recebe os dados
por `RotaMemoriaRepository`. Portanto, executar o importador preenche o banco,
mas a tela atual não passa automaticamente a ler essas novas tabelas. A
substituição desse repositório por um repositório SQLite é uma etapa separada
para conectar a interface aos dados persistidos.
