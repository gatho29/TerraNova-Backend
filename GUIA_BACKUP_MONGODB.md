# 📦 Guía Rápida: Hacer Backup desde MongoDB Compass

Esta guía te muestra cómo hacer un backup de tu base de datos MongoDB desde MongoDB Compass para subirlo a GitHub.

## 🚀 Pasos Rápidos

### 1. Conectar MongoDB Compass a tu Base de Datos

1. Abre **MongoDB Compass**
2. En la barra de conexión, pega tu cadena de conexión:
   ```
   mongodb+srv://guerranell25_db_user:lxhZ4U2E6rA6VdrM@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority
   ```
3. Credenciales
Usuario: guerranell25_db_user
Contraseña: lxhZ4U2E6rA6VdrM

4. Haz clic en **"Connect"**


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