# Script para importar base de datos MongoDB desde archivos JSON
# Uso: .\database\scripts\import-database.ps1 -Collection Properties -FilePath database\exports\Properties.json

param(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString = "",
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseName = "PropertyDB",
    
    [Parameter(Mandatory=$false)]
    [string]$Collection = "",
    
    [Parameter(Mandatory=$false)]
    [string]$FilePath = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Importador de Base de Datos MongoDB" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si mongoimport está disponible
$mongoimportPath = Get-Command mongoimport -ErrorAction SilentlyContinue

if (-not $mongoimportPath) {
    Write-Host "⚠️ MongoDB Database Tools no está instalado." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternativa: Importar manualmente desde MongoDB Compass:" -ForegroundColor Yellow
    Write-Host "1. Abre MongoDB Compass" -ForegroundColor White
    Write-Host "2. Conecta a tu base de datos" -ForegroundColor White
    Write-Host "3. Selecciona la base de datos $DatabaseName" -ForegroundColor White
    Write-Host "4. Haz clic en 'ADD DATA' → 'Import File'" -ForegroundColor White
    Write-Host "5. Selecciona el archivo JSON y la colección destino" -ForegroundColor White
    Write-Host ""
    Write-Host "Para instalar MongoDB Database Tools:" -ForegroundColor Yellow
    Write-Host "https://www.mongodb.com/try/download/database-tools" -ForegroundColor Blue
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

# Si se proporciona un archivo específico, importarlo
if (-not [string]::IsNullOrWhiteSpace($FilePath)) {
    if (-not (Test-Path $FilePath)) {
        Write-Host "❌ Error: El archivo no existe: $FilePath" -ForegroundColor Red
        exit 1
    }
    
    if ([string]::IsNullOrWhiteSpace($Collection)) {
        # Intentar obtener el nombre de la colección del nombre del archivo
        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($FilePath)
        $Collection = $fileName -replace '_\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}$', ''
        Write-Host "Colección detectada del nombre del archivo: $Collection" -ForegroundColor Gray
    }
    
    Write-Host "Importando archivo: $FilePath" -ForegroundColor Cyan
    Write-Host "Colección: $Collection" -ForegroundColor Gray
    Write-Host "Base de datos: $DatabaseName" -ForegroundColor Gray
    Write-Host ""
    
    $importUri = "$ConnectionString/$DatabaseName"
    
    try {
        & mongoimport --uri=$importUri --collection=$Collection --type=json --file=$FilePath --drop
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Importación exitosa!" -ForegroundColor Green
        } else {
            Write-Host "❌ Error al importar" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "❌ Error: $_" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host "⚠️ No se especificó un archivo para importar." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Uso del script:" -ForegroundColor Yellow
    Write-Host "  .\database\scripts\import-database.ps1 -FilePath database\exports\Properties.json -Collection Properties" -ForegroundColor White
    Write-Host ""
    exit 1
}

