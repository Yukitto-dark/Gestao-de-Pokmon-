# Cadastro de habilidades da Pokédex

Este pacote cadastra no SQLite as habilidades normais dos Pokémon #001–#386, limitadas às habilidades introduzidas na Geração III.

## Compatibilidade com o modelo existente

O importador respeita as classes e nomes usados em `Database/database.cs`:

- `PokemonAbility`: `Id`, `Name`, `Description` → tabela `Abilities`.
- `PokemonFormAbility`: `Id`, `PokemonFormId`, `AbilityId`, `Slot` → tabela `FormAbilities`.
- `AbilitySlot` do namespace `GestaoDePokemon.Database`: `Slot1 = 0`, `Slot2 = 1`, `Hidden = 2`.
- Vincula as habilidades à forma `Normal` em `PokemonForms`, relacionando pelo `PokemonId` e pelo `NationalDexNumber`.

Nenhuma classe C# é alterada e nenhuma tabela/coluna nova é criada. A enumeração `AbilitySlot` que aparece em `model/Enums` possui nomes `First`/`Second`, mas os valores numéricos são os mesmos (0/1); o banco armazena o valor inteiro do enum.

## Escopo de dados

- Pokémon #001–#386.
- Habilidades introduzidas na Geração III (lista de habilidades da PokéAPI `generation/3`).
- Somente os slots normais 1 e 2.
- Habilidades ocultas não são importadas, pois foram introduzidas na Geração V.
- A descrição é obtida da entrada em inglês da PokéAPI (`short_effect`, com fallback para `effect`/texto de sabor). A classe atual não tem campo para idioma ou geração.

## Execução

Na raiz do projeto:

```bash
python3 scripts/importar_habilidades.py \
  --database "$HOME/.wine-dotnet-10/drive_c/users/jose/AppData/Local/GestaoDePokemon/gestaopokemon.db"
```

A primeira execução exige internet. A API é armazenada em cache em `~/.cache/gestaopokemon/pokeapi-habilidades`. O banco recebe backup automático antes da gravação. O arquivo `Database/Seed/abilities-seed.json` também é gerado para versionamento no Git.

Teste interno sem rede e sem alterar o banco real:

```bash
python3 scripts/importar_habilidades.py --self-test
```

Prévia dos dados sem alteração do banco:

```bash
python3 scripts/importar_habilidades.py --dry-run
```

## Verificação SQL

Consulte `consultas_habilidades.sql`.
