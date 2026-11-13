# Sprites locais de Pokémon

Mantenha as três pastas nesta localização, com arquivos nomeados pelo número da Pokédex nacional:

- `frontNormal/<numero>.png`: sprite frontal normal, exibido no painel de detalhes.
- `frontShiny/<numero>.png`: sprite frontal shiny, alternado pelo botão `Shine`.
- `mini/<numero>.png`: sprite pequeno usado nas células da Box.

A grade da Box usa exclusivamente `mini`. Ao selecionar uma espécie, o painel de detalhes carrega `frontNormal`; clicar no botão transparente sobre o ícone shiny alterna entre `frontNormal` e `frontShiny`. Se o arquivo shiny não existir, o repositório usa o sprite frontal normal como alternativa.

Os arquivos são copiados para a pasta de saída/publicação pelo `GestaoDePokemon.csproj`, e os caminhos relativos são cadastrados na tabela `Sprites` pelo `DatabaseInitializer`.
