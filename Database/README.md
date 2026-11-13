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
