# Script para exportar base de datos MongoDB desde Compass
# Uso: .\database\scripts\export-database.ps1

param(
    [string]$ConnectionString = "",
    [string]$DatabaseName = "PropertyDB",
    [string]$OutputPath = "database\exports",
    [string[]]$Collections = @("Properties", "Owners", "PropertyImages", "PropertyTraces")
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Exportador de Base de Datos MongoDB" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si mongodump está disponible
$mongodumpPath = Get-Command mongodump -ErrorAction SilentlyContinue

if (-not $mongodumpPath) {
    Write-Host "⚠️ MongoDB Database Tools no está instalado." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Opciones disponibles:" -ForegroundColor Yellow
    Write-Host "1. Exportar manualmente desde MongoDB Compass:" -ForegroundColor White
    Write-Host "   - Abre MongoDB Compass" -ForegroundColor Gray
    Write-Host "   - Conecta a tu base de datos" -ForegroundColor Gray
    Write-Host "   - Para cada colección, haz clic en 'Export Collection'" -ForegroundColor Gray
    Write-Host "   - Guarda los archivos JSON en: $OutputPath" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Instalar MongoDB Database Tools:" -ForegroundColor White
    Write-Host "   https://www.mongodb.com/try/download/database-tools" -ForegroundColor Blue
    Write-Host ""
    Write-Host "3. Usar el script de exportación manual:" -ForegroundColor White
    Write-Host "   Ver: database\backups\README.md" -ForegroundColor Gray
    Write-Host ""
    exit 1
}

# Solicitar cadena de conexión si no se proporciona
if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    Write-Host "Por favor, ingresa la cadena de conexión MongoDB:" -ForegroundColor Yellow
    Write-Host "Ejemplo: mongodb+srv://usuario:password@cluster.mongodb.net/$DatabaseName" -ForegroundColor Gray
    $ConnectionString = Read-Host "Cadena de conexión"
    
    if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
        Write-Host "❌ Error: La cadena de conexión es requerida." -ForegroundColor Red
        exit 1
    }
}

# Crear directorio de salida si no existe
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "✅ Directorio creado: $OutputPath" -ForegroundColor Green
}

Write-Host ""
Write-Host "Exportando colecciones..." -ForegroundColor Cyan
Write-Host "Base de datos: $DatabaseName" -ForegroundColor Gray
Write-Host "Directorio de salida: $OutputPath" -ForegroundColor Gray
Write-Host ""

$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$successCount = 0
$failCount = 0

foreach ($collection in $Collections) {
    Write-Host "Exportando colección: $collection" -ForegroundColor Yellow
    
    $outputFile = Join-Path $OutputPath "$collection`_$timestamp.json"
    
    try {
        # Usar mongoexport si está disponible, de lo contrario usar mongodump
        $mongoexportPath = Get-Command mongoexport -ErrorAction SilentlyContinue
        
        if ($mongoexportPath) {
            $exportUri = "$ConnectionString/$DatabaseName"
            & mongoexport --uri=$exportUri --collection=$collection --type=json --out=$outputFile
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  ✅ Exportado exitosamente: $outputFile" -ForegroundColor Green
                $successCount++
            } else {
                Write-Host "  ❌ Error al exportar: $collection" -ForegroundColor Red
                $failCount++
            }
        } else {
            Write-Host "  ⚠️ mongoexport no disponible. Usa MongoDB Compass para exportar manualmente." -ForegroundColor Yellow
            $failCount++
        }
    }
    catch {
        Write-Host "  ❌ Error: $_" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Resumen de Exportación" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ Exitosos: $successCount" -ForegroundColor Green
Write-Host "❌ Fallidos: $failCount" -ForegroundColor Red
Write-Host ""

if ($successCount -gt 0) {
    Write-Host "Los archivos se guardaron en: $OutputPath" -ForegroundColor Green
}

