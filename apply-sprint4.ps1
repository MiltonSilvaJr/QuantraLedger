# apply-sprint4.ps1
$ErrorActionPreference = 'Stop'

# Lista de arquivos a corrigir
$files = @(
  'src\Quantra.Transaction\Program.cs',
  'src\Quantra.Onboarding\Program.cs',
  'src\Quantra.Console\Program.cs'
)

foreach ($file in $files) {
  if (Test-Path $file) {
    # Lê todo o conteúdo
    $content = Get-Content $file

    # Separa as linhas de using e as demais
    $usings = $content | Where-Object { $_.TrimStart().StartsWith('using ') }
    $others = $content | Where-Object { -not $_.TrimStart().StartsWith('using ') }

    # Remonta com os usings no topo e uma linha em branco depois
    $newContent = $usings + '' + $others

    # Grava de volta
    $newContent | Set-Content $file -Encoding UTF8
    Write-Host "Corrigido: $file"
  }
  else {
    Write-Warning "Arquivo não encontrado: $file"
  }
}
