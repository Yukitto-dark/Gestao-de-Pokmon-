# Regiões, rotas e encontros — SistemaDeGestaoDePokemon

Este pacote adiciona ao SQLite as rotas numeradas de Kanto, Johto, Hoenn,
Sinnoh, Unova, Kalos, Alola e Galar. Cada rota é dividida em suas áreas da PokéAPI; cada encontro
armazena espécie, método, níveis mínimo/máximo, chance, versão do jogo e
condições do encontro.

## Escopo

- Kanto: rotas 1–28.
- Johto: rotas 29–46.
- Hoenn: rotas 101–134.
- Versões consultadas para Kanto, Johto e Hoenn: Red, Blue, Yellow, Gold,
  Silver, Crystal, FireRed, LeafGreen, Ruby, Sapphire e Emerald.
- Sinnoh: rotas 201–230 (Diamond, Pearl e Platinum).
- Unova: rotas 1–23 (Black, White, Black 2 e White 2).
- Kalos: rotas 1–22 (X e Y).
- Alola: rotas 1–17 (Sun, Moon, Ultra Sun e Ultra Moon).
- Galar: rotas 1–10 (Sword e Shield).
- Paldea: 32 locais do jogo base de Scarlet/Violet (províncias, lagos, mares, cavernas,
  cidades e Área Zero), numerados de 1 a 32 só para a ordem e exibidos pelo nome. Cada local tem
  uma área. Fonte: `encounter_wild_paldea.pkl` e `text_sv_00000_en.txt` do PKHeX (a PokéAPI não
  tem encontros de Scarlet/Violet). Ficam de fora Kitakami e Blueberry Academy (DLCs, fora de
  Paldea) e as áreas "cruzadas", que repetem Pokémon do local vizinho que passam da divisa. Os
  identificadores e ids de local seguem a PokéAPI quando o local existe lá.
- Hisui: as cinco grandes áreas de Legends: Arceus (Obsidian Fieldlands, Crimson Mirelands,
  Cobalt Coastlands, Coronet Highlands e Alabaster Icelands) são as rotas, e os sublocais de cada
  uma (Horseshoe Plains, Lake Verity...) são as áreas. Fonte: `encounter_la.pkl` e
  `text_la_00000_en.txt` do PKHeX. Entram os encontros normais (`overworld`), de marcos
  (`landmark`) e de distorções espaço-temporais (`space-time-distortion`); os surtos em massa,
  aleatórios, ficam de fora. Pokémon alfa têm a condição `alpha`.
- Em Paldea e Hisui não há chance de encontro por slot (`chancePercent` = 0). Em Paldea, as
  restrições de horário viram condições (`time-day`, `time-night`, `time-dusk`, `time-morning`);
  as versões não são separadas (`scarlet-violet`).
- As rotas de Sinnoh a Galar foram geradas dos CSVs da PokéAPI
  (`data/v2/csv`: `locations`, `location_areas`, `encounters`,
  `encounter_slots` e tabelas relacionadas), no mesmo formato do seed original,
  com encontros idênticos (mesmos dados em outro slot) registrados uma vez só.
- A importação é incremental: rotas já gravadas (mesma região e número) não
  são alteradas, e as que faltam são acrescentadas na abertura do aplicativo.
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

## Tela de regiões

`RegioesController` usa `RotaSqliteRepository` e lê diretamente as tabelas
`Routes`, `RouteAreas` e `RoutePokemonEncounters`. O seed local
`Data/Persistence/Seed/rotas-seed.json` é importado automaticamente quando a
tabela `Routes` está vazia. O importador Python continua sendo a forma de
atualizar os encontros a partir da PokéAPI.
