# 📦 Backups de Base de Datos MongoDB

Este directorio contiene los backups de la base de datos `PropertyDB`.

## ⚠️ Importante

- **NO subir backups con datos sensibles o información personal** a GitHub
- Los backups grandes pueden ralentizar el repositorio
- Usa backups de ejemplo para documentación si es necesario

## 📋 Guía para Hacer Backup desde MongoDB Compass

### Opción 1: Exportar Colecciones Individuales (JSON)

1. **Conectar MongoDB Compass a tu base de datos:**
   - Abre MongoDB Compass
   - Conecta usando la cadena de conexión:
     ```
     mongodb+srv://guerranell25_db_user:lxhZ4U2E6rA6VdrM@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority
     ```

2. **Exportar cada colección:**
   - Selecciona la base de datos `PropertyDB`
   - Haz clic en una colección (ej: `Properties`)
   - Haz clic en el botón **"Export Collection"** (icono de descarga)
   - Selecciona formato: **JSON**
   - Selecciona campos: **All Fields** o selecciona campos específicos
   - Guarda el archivo en: `database/exports/Properties.json`

3. **Repite para todas las colecciones:**
   - `Properties`
   - `Owners`
   - `PropertyImages`
   - `PropertyTraces`

### Opción 2: Usar mongodump (Línea de Comandos)

Si tienes MongoDB Database Tools instalado:

```bash
# Exportar toda la base de datos
mongodump --uri="mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB" --out=./database/backups/backup-$(Get-Date -Format "yyyy-MM-dd")

# Exportar una colección específica
mongodump --uri="mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB" --collection=Properties --out=./database/backups
```

### Opción 3: Usar el Script PowerShell

Ejecuta el script `export-database.ps1` desde la raíz del proyecto:

```powershell
.\database\scripts\export-database.ps1
```

## 📂 Estructura de Archivos

```
database/
├── backups/          # Backups completos (NO subir a GitHub si contienen datos reales)
│   └── README.md
├── exports/          # Exports JSON de colecciones individuales
│   └── README.md
└── scripts/          # Scripts de backup/restore
    ├── export-database.ps1
    └── import-database.ps1
```

## 🔄 Restaurar un Backup

Para restaurar un backup desde MongoDB Compass:

1. **Importar colección desde JSON:**
   - Abre MongoDB Compass
   - Selecciona la base de datos `PropertyDB`
   - Haz clic en **"ADD DATA"** → **"Import File"**
   - Selecciona el archivo JSON
   - Selecciona la colección destino
   - Haz clic en **"Import"**

## 📝 Notas

- Los backups de producción deben mantenerse en un lugar seguro fuera del repositorio
- Usa variables de entorno para las credenciales de conexión
- Considera usar MongoDB Atlas Automated Backups para backups de producción

