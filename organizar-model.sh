#!/usr/bin/env bash
set -Eeuo pipefail

# Execute este script na raiz do projeto, ao lado da pasta "model".
# Ele apenas move arquivos .cs; não altera namespaces nem o conteúdo dos arquivos.

PROJECT_ROOT="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
MODEL_DIR="$PROJECT_ROOT/model"

if [[ ! -d "$MODEL_DIR" ]]; then
    echo "Erro: não encontrei a pasta '$MODEL_DIR'." >&2
    echo "Coloque este script na raiz do projeto, ao lado da pasta model/." >&2
    exit 1
fi

ENTITIES=(
    Pokemon.cs
    PokemonForm.cs
    PokemonBuild.cs
    PokemonTeam.cs
    PokemonFavorites.cs
    PokemonEvolution.cs
    Region.cs
    PokemonMove.cs
    LearnableMove.cs
    PokemonAbility.cs
    PokemonFormAbility.cs
    PokemonSprite.cs
)

VALUE_OBJECTS=(
    BaseStats.cs
    IndividualValues.cs
    EffortValues.cs
    NatureModifiers.cs
)

ENUMS=(
    AbilitySlot.cs
    EggGroup.cs
    EvolutionMethod.cs
    EvolutionTime.cs
    MoveLearnMethod.cs
    PokemonGender.cs
    PokemonNature.cs
    PokemonStat.cs
    PokemonType.cs
)

RULES=(
    PokemonNatureRules.cs
)

# Faz uma verificação antecipada para não sobrescrever arquivos existentes.
check_conflicts() {
    local folder="$1"
    shift
    local file
    local destination

    for file in "$@"; do
        destination="$MODEL_DIR/$folder/$file"

        if [[ -f "$MODEL_DIR/$file" && -e "$destination" ]]; then
            echo "Erro: '$destination' já existe. Nada foi movido." >&2
            exit 1
        fi
    done
}

check_conflicts "Entities" "${ENTITIES[@]}"
check_conflicts "ValueObjects" "${VALUE_OBJECTS[@]}"
check_conflicts "Enums" "${ENUMS[@]}"
check_conflicts "Rules" "${RULES[@]}"

mkdir -p \
    "$MODEL_DIR/Entities" \
    "$MODEL_DIR/ValueObjects" \
    "$MODEL_DIR/Enums" \
    "$MODEL_DIR/Rules"

move_files() {
    local folder="$1"
    shift
    local file
    local source
    local destination

    for file in "$@"; do
        source="$MODEL_DIR/$file"
        destination="$MODEL_DIR/$folder/$file"

        if [[ -f "$source" ]]; then
            mv -- "$source" "$destination"
            echo "Movido: model/$file -> model/$folder/$file"
        elif [[ -f "$destination" ]]; then
            echo "Já organizado: model/$folder/$file"
        else
            echo "Ignorado (não encontrado): model/$file"
        fi
    done
}

move_files "Entities" "${ENTITIES[@]}"
move_files "ValueObjects" "${VALUE_OBJECTS[@]}"
move_files "Enums" "${ENUMS[@]}"
move_files "Rules" "${RULES[@]}"

# Move o serviço de linha evolutiva para fora da Model, caso esteja nela.
if [[ -f "$MODEL_DIR/PokemonEvolutionLineService.cs" ]]; then
    mkdir -p "$PROJECT_ROOT/Services"
    if [[ -e "$PROJECT_ROOT/Services/PokemonEvolutionLineService.cs" ]]; then
        echo "Erro: Services/PokemonEvolutionLineService.cs já existe." >&2
        echo "O arquivo na pasta model não foi movido." >&2
        exit 1
    fi
    mv -- "$MODEL_DIR/PokemonEvolutionLineService.cs" \
        "$PROJECT_ROOT/Services/PokemonEvolutionLineService.cs"
    echo "Movido: model/PokemonEvolutionLineService.cs -> Services/PokemonEvolutionLineService.cs"
fi

# Mostra arquivos C# que não estavam no mapeamento, sem movê-los automaticamente.
shopt -s nullglob
remaining=("$MODEL_DIR"/*.cs)
if (( ${#remaining[@]} > 0 )); then
    echo
    echo "Atenção: estes arquivos .cs permaneceram diretamente em model/:"
    for file in "${remaining[@]}"; do
        echo "  - ${file##*/}"
    done
    echo "Revise-os e escolha uma pasta apropriada, se necessário."
fi

cat <<'MSG'

Organização concluída.
Os namespaces foram preservados como GestaoDePokemon.model.
Agora execute na raiz do projeto:

    dotnet clean
    dotnet restore
    dotnet build GestaoDePokemon.slnx

Observação: o arquivo PokemonMove.cs também contém o enum MoveCategory,
por isso ambos permanecem juntos em Entities/.
MSG
