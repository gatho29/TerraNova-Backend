# 📦 Guía Rápida: Hacer Backup desde MongoDB Compass

Esta guía te muestra cómo hacer un backup de tu base de datos MongoDB desde MongoDB Compass para subirlo a GitHub.

## 🚀 Pasos Rápidos

### 1. Conectar MongoDB Compass a tu Base de Datos

1. Abre **MongoDB Compass**
2. En la barra de conexión, pega tu cadena de conexión:
   ```
   mongodb+srv://guerranell25_db_user:lxhZ4U2E6rA6VdrM@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority
   ```
3. Haz clic en **"Connect"**

### 2. Exportar Colecciones a JSON

Para cada colección que quieras exportar:

1. **Selecciona la base de datos** `PropertyDB` en el panel izquierdo
2. **Haz clic en la colección** que quieres exportar (ej: `Properties`)
3. En la parte superior, verás un botón de **descarga/export** (icono de flecha hacia abajo o "Export Collection")
4. Haz clic en **"Export Collection"**
5. Selecciona:
   - **Formato:** JSON
   - **Campos:** All Fields (o selecciona campos específicos)
6. **Guarda el archivo** en: `database/exports/NombreColeccion.json`
   - Ejemplo: `database/exports/Properties.json`

### 3. Repite para Todas las Colecciones

Exporta las siguientes colecciones:

- ✅ `Properties` → `database/exports/Properties.json`
- ✅ `Owners` → `database/exports/Owners.json`
- ✅ `PropertyImages` → `database/exports/PropertyImages.json`
- ✅ `PropertyTraces` → `database/exports/PropertyTraces.json`

### 4. ⚠️ IMPORTANTE: Verificar Archivos Antes de Subir

**NO subas archivos con datos sensibles** (información personal, números de teléfono reales, direcciones reales, etc.) a GitHub.

Si necesitas subir datos de ejemplo:
- Usa nombres como `Properties.example.json`
- Estos archivos SÍ se subirán a GitHub (ver `.gitignore`)
- Reemplaza datos reales con datos ficticios

### 5. Agregar Archivos a Git

Una vez que tengas los archivos exportados:

```bash
# Agregar los archivos
git add database/exports/

# Hacer commit
git commit -m "Add database exports"

# Subir a GitHub
git push
```

## 📋 Alternativa: Usar Scripts PowerShell

Si prefieres usar scripts automáticos:

```powershell
# Exportar toda la base de datos
.\database\scripts\export-database.ps1

# O especificar parámetros
.\database\scripts\export-database.ps1 -ConnectionString "tu_cadena_de_conexion"
```

**Nota:** Necesitas tener MongoDB Database Tools instalado para usar los scripts.

## 🔄 Restaurar un Backup

Para restaurar un backup:

1. Abre MongoDB Compass
2. Conecta a tu base de datos
3. Selecciona la base de datos `PropertyDB`
4. Haz clic en **"ADD DATA"** → **"Import File"**
5. Selecciona el archivo JSON
6. Selecciona la colección destino
7. Haz clic en **"Import"**

## 📂 Estructura de Archivos

```
database/
├── backups/
│   └── README.md                    # Guía completa de backups
├── exports/
│   ├── README.md                    # Información sobre exports
│   ├── Properties.example.json      # ✅ Ejemplo (se sube a GitHub)
│   ├── Properties.json              # ❌ Datos reales (NO se sube)
│   └── ...                          # Otras colecciones
└── scripts/
    ├── export-database.ps1          # Script de exportación
    └── import-database.ps1          # Script de importación
```

## ❓ Preguntas Frecuentes

**¿Puedo subir mis datos reales a GitHub?**
- No recomendado. Si necesitas hacerlo, asegúrate de que no contengan información sensible.

**¿Cómo sé si un archivo se subirá a GitHub?**
- Los archivos `*.example.json` SÍ se suben
- Los archivos `*.json` normales NO se suben (están en `.gitignore`)

**¿Qué pasa si quiero subir datos reales?**
- Renombra el archivo a `*.example.json` o elimina la regla del `.gitignore`
- ⚠️ Ten cuidado con información sensible

## 📚 Más Información

- Ver `database/backups/README.md` para guía completa
- Ver `database/exports/README.md` para información sobre exports

