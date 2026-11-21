# Banco de dados e sprites locais

- `AppDbContext.cs`: configuração do Entity Framework Core com SQLite.
- `DatabaseInitializer.cs`: criação das tabelas e seed incremental das regiões, espécies e sprites locais.
- `Seed/pokemon-seed.json`: conjunto inicial de 386 espécies convertido para as entidades oficiais de `model/Entities`.

As entidades de domínio ficam exclusivamente em `model/Entities`. Os enums ficam em `model/Enums` e os objetos de valor em `model/ValueObjects`.

## Localização do banco

O SQLite é criado em `%LOCALAPPDATA%/GestaoDePokemon/gestaopokemon-gen3.db` no Windows e no prefixo do Wine. O banco existente não é apagado nem recriado pelo seed de sprites.

## Sprites locais

Os sprites front_default de #001 a #386 ficam em `Data/images/pokemon/` com os nomes `1.png` até `386.png`.

O `.csproj` copia os PNGs para `Data/images/pokemon/` ao lado do executável em Debug, Release e Publish. Na inicialização, `DatabaseInitializer` procura os arquivos copiados e cria ou atualiza os registros de `PokemonSprite` para as formas `Normal`. Os campos `BoxDefault` e `ExhibitionDefault` guardam caminhos relativos, por exemplo `Data/images/pokemon/1.png`. `PokemonSqliteRepository` resolve esses caminhos para URIs `file://` absolutas para a interface WPF.

Se o aplicativo for aberto antes de os PNGs estarem disponíveis, nenhum caminho inexistente será registrado. Depois de adicionar os arquivos e compilar/publicar novamente, abra a aplicação outra vez: o seed incremental preencherá a tabela `Sprites` sem duplicar Pokémon ou formas.

Fonte dos sprites: https://github.com/PokeAPI/sprites/tree/master/sprites/pokemon. Consulte também `Data/images/pokemon/README.md` para as informações de licença e direitos de terceiros.

## Repositórios

`AppServices` utiliza `PokemonSqliteRepository`, `FavoritosSqliteRepository` e `RotaSqliteRepository`. As rotas são lidas das tabelas `Routes`, `RouteAreas` e `RoutePokemonEncounters`.

O seed inicial de rotas fica em `Seed/rotas-seed.json` e é importado quando a tabela `Routes` está vazia. O importador Python descrito em `README_ROTAS.md` continua disponível para atualizar os encontros a partir da PokéAPI.

O projeto usa `EnsureCreated()` enquanto o modelo inicial é estabilizado. Antes de alterar a estrutura de um banco já em uso, adote migrations do EF Core; `EnsureCreated()` não atualiza automaticamente tabelas existentes. Para bancos já existentes, `DatabaseInitializer` cria as tabelas de rotas com `CREATE TABLE IF NOT EXISTS`.

## Base Stats, movimentos e learnsets

- Os Base Stats já estão no `pokemon-seed.json`, dentro de `Form.Stats`. Como `BaseStats` é um `readonly record struct`, o EF Core persiste esse objeto na coluna `Stats` de `PokemonForms` como JSON. Não é necessária uma tabela redundante só para os seis atributos.
- `DatabaseInitializer.EnsureBaseStats` verifica as 386 formas e repara dados ausentes/inválidos a partir do seed de espécies, sem sobrescrever valores completos.
- `PokemonMove` guarda o catálogo de movimentos; `LearnableMove` liga cada forma aos movimentos que pode aprender, com método, nível e grupo de jogos.
- Os dados locais já estão versionados em `move-catalog-seed.json` e `pokemon-learnsets-seed.json` em `Data/Persistence/Seed/`.
- O recorte de learnsets inclui os jogos principais Red/Blue, Yellow, Gold/Silver, Crystal, Ruby/Sapphire, Emerald e FireRed/LeafGreen. O campo `GameVersion` guarda o identificador do grupo de versão (por exemplo `emerald`).
- `MoveLearnMethod.TM` recebe o método `machine` da PokeAPI, que representa máquinas técnicas (TM/HM) no modelo atual. O método especial `light-ball-egg` é tratado como `Egg` (caso do Volt Tackle). Métodos de spin-offs e de gerações posteriores ficam fora deste recorte.
- Como as regras das três primeiras gerações classificam golpes físicos/especiais pelo tipo, o gerador calcula `MoveCategory` pela tipagem antiga (e mantém golpes de status como `Status`). Os valores de Power/Accuracy/PP vêm do catálogo atual da PokeAPI e não são uma tabela histórica individual por jogo.
- Após gerar os JSONs, compile e abra a aplicação. O `DatabaseInitializer` importa o catálogo e os learnsets no SQLite de forma incremental, sem apagar favoritos, times, Pokémon ou sprites.

Fonte dos dados de movimentos e learnsets: CSVs do repositório `PokeAPI/pokeapi`, em `data/v2/csv/`. Os JSONs ficam versionados no projeto para não ser necessário baixar os CSVs em cada inicialização.


## Relacionamentos de evolução

- `PokemonEvolution` representa uma relação direta entre uma espécie de origem (`FromPokemonId`) e uma espécie de destino (`ToPokemonId`). A tabela `Evolutions` possui chaves estrangeiras para as duas espécies.
- Os dados da Pokédex #001–#386 já estão versionados em `Data/Persistence/Seed/pokemon-evolutions-seed.json`, gerado a partir de `pokemon_species.csv`, `pokemon_evolution.csv` e tabelas auxiliares do repositório oficial `PokeAPI/pokeapi`.
- O seed considera os grupos de jogos principais das gerações I–III: Red/Blue, Yellow, Gold/Silver, Crystal, Ruby/Sapphire, Emerald e FireRed/LeafGreen. Quando a mesma regra aparece em mais de um grupo, os nomes são agrupados em `GameVersion` para evitar duplicar a relação.
- O `DatabaseInitializer` importa as relações de forma incremental. Ao abrir o aplicativo, cada relação liga o registro de origem ao registro de destino por suas chaves estrangeiras; executar o seed novamente não duplica os vínculos nem apaga registros existentes.
- O modelo atual armazena nível, item, horário, amizade, beleza, gênero, local, movimento exigido e grupos de jogos. Algumas condições especiais mais novas ou muito específicas da PokeAPI (por exemplo, comparação entre Attack/Defense e expressões internas de condições) ainda não têm campos próprios em `PokemonEvolution`; o gerador avisa quando encontra essas condições e preserva o vínculo entre as espécies.

Fonte dos dados de evolução: `PokeAPI/pokeapi`, arquivos `data/v2/csv/pokemon_species.csv`, `pokemon_evolution.csv`, `evolution_triggers.csv`, `version_groups.csv`, `items.csv`, `locations.csv` e `moves.csv`.
