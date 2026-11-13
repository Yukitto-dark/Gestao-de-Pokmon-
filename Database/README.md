# Banco de dados e sprites locais

- `AppDbContext.cs`: configuração do Entity Framework Core com SQLite.
- `DatabaseInitializer.cs`: criação das tabelas e seed incremental das regiões, espécies e sprites locais.
- `Seed/pokemon-seed.json`: conjunto inicial de 386 espécies convertido para as entidades oficiais de `model/Entities`.

As entidades de domínio ficam exclusivamente em `model/Entities`. Os enums ficam em `model/Enums` e os objetos de valor em `model/ValueObjects`.

## Localização do banco

O SQLite é criado em `%LOCALAPPDATA%/GestaoDePokemon/gestaopokemon.db` no Windows e no prefixo do Wine. O banco existente não é apagado nem recriado pelo seed de sprites.

## Sprites locais

Os sprites front_default de #001 a #386 ficam em `Assets/Pokemon/` com os nomes `1.png` até `386.png`. Para baixá-los no Linux, na raiz do projeto, execute:

```bash
./scripts/download-pokemon-sprites.sh
```

O script baixa os arquivos do repositório `PokeAPI/sprites`, valida a assinatura PNG e pode ser executado novamente sem baixar arquivos já válidos.

O `.csproj` copia os PNGs para `Assets/Pokemon/` ao lado do executável em Debug, Release e Publish. Na inicialização, `DatabaseInitializer` procura os arquivos copiados e cria ou atualiza os registros de `PokemonSprite` para as formas `Normal`. Os campos `BoxDefault` e `ExhibitionDefault` guardam caminhos relativos, por exemplo `Assets/Pokemon/1.png`. `PokemonSqliteRepository` resolve esses caminhos para URIs `file://` absolutas para a interface WPF.

Se o aplicativo for aberto antes de baixar os sprites, nenhum caminho inexistente será registrado. Depois de executar o script e compilar/publicar novamente, abra a aplicação outra vez: o seed incremental preencherá a tabela `Sprites` sem duplicar Pokémon ou formas.

Fonte dos sprites: https://github.com/PokeAPI/sprites/tree/master/sprites/pokemon. Consulte também `Assets/Pokemon/README.md` para as informações de licença e direitos de terceiros.

## Repositórios

`AppServices` utiliza `PokemonSqliteRepository` e `FavoritosSqliteRepository`. Rotas ainda usam repositório em memória, pois a entidade/tabela de rotas não foi migrada nesta etapa.

O projeto usa `EnsureCreated()` enquanto o modelo inicial é estabilizado. Antes de alterar a estrutura de um banco já em uso, adote migrations do EF Core; `EnsureCreated()` não atualiza automaticamente tabelas existentes.

## Base Stats, movimentos e learnsets

- Os Base Stats já estão no `pokemon-seed.json`, dentro de `Form.Stats`. Como `BaseStats` é um `readonly record struct`, o EF Core persiste esse objeto na coluna `Stats` de `PokemonForms` como JSON. Não é necessária uma tabela redundante só para os seis atributos.
- `DatabaseInitializer.EnsureBaseStats` verifica as 386 formas e repara dados ausentes/inválidos a partir do seed de espécies, sem sobrescrever valores completos.
- `PokemonMove` guarda o catálogo de movimentos; `LearnableMove` liga cada forma aos movimentos que pode aprender, com método, nível e grupo de jogos.
- Para gerar os dados locais, execute na raiz do projeto: `python3 scripts/gerar_seed_moves.py`. O script baixa os CSVs da PokeAPI e gera `move-catalog-seed.json` e `pokemon-learnsets-seed.json` em `Database/Seed/`.
- O recorte de learnsets inclui os jogos principais Red/Blue, Yellow, Gold/Silver, Crystal, Ruby/Sapphire, Emerald e FireRed/LeafGreen. O campo `GameVersion` guarda o identificador do grupo de versão (por exemplo `emerald`).
- `MoveLearnMethod.TM` recebe o método `machine` da PokeAPI, que representa máquinas técnicas (TM/HM) no modelo atual. O método especial `light-ball-egg` é tratado como `Egg` (caso do Volt Tackle). Métodos de spin-offs e de gerações posteriores ficam fora deste recorte.
- Como as regras das três primeiras gerações classificam golpes físicos/especiais pelo tipo, o gerador calcula `MoveCategory` pela tipagem antiga (e mantém golpes de status como `Status`). Os valores de Power/Accuracy/PP vêm do catálogo atual da PokeAPI e não são uma tabela histórica individual por jogo.
- Após gerar os JSONs, compile e abra a aplicação. O `DatabaseInitializer` importa o catálogo e os learnsets no SQLite de forma incremental, sem apagar favoritos, times, Pokémon ou sprites.

Fonte dos dados de movimentos e learnsets: CSVs do repositório `PokeAPI/pokeapi`, em `data/v2/csv/`. O script guarda os JSONs gerados no próprio projeto para que possam ser versionados no Git e não seja necessário baixar os CSVs novamente em cada inicialização.
