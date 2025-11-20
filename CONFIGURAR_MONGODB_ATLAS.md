# 🔗 Guía de Conexión a MongoDB Atlas

Esta guía te ayudará a conectarte a la base de datos MongoDB Atlas existente del proyecto TerraNova API.

---

## 📋 Información de la Base de Datos

- **Base de datos:** `PropertyDB`
- **Colecciones:**
  - `Properties` - Propiedades inmobiliarias
  - `Owners` - Propietarios
  - `PropertyImages` - Imágenes de propiedades
  - `PropertyTraces` - Historial de transacciones

---

## 🔑 Paso 1: Obtener Credenciales de Conexión

Para conectarte a la base de datos, necesitas obtener las credenciales de acceso. Contacta al administrador del proyecto para obtener:

1. **Cadena de conexión completa** o:
   - Usuario de base de datos
   - Contraseña
   - URL del cluster

**Ejemplo de cadena de conexión:**
```
mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority
```

---

## ⚙️ Paso 2: Configurar la Aplicación

### Opción A: Usar appsettings.json (Desarrollo)

1. **Abre el archivo de configuración:**
   - `src/PropertyAPI/appsettings.json`
   - O `src/PropertyAPI/appsettings.Development.json`

2. **Actualiza la cadena de conexión:**
   
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority"
     },
     "MongoDB": {
       "DatabaseName": "PropertyDB"
     }
   }
   ```

3. **Reemplaza** `usuario:password` con las credenciales reales proporcionadas

4. **Guarda el archivo**

> **⚠️ Importante:** No subas este archivo con credenciales reales a Git. Usa User Secrets o variables de entorno.

### Opción B: Usar User Secrets (Recomendado)

1. **Inicializa User Secrets:**
   ```bash
   cd src\PropertyAPI
   dotnet user-secrets init
   ```

2. **Guarda la cadena de conexión:**
   ```bash
   dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority"
   ```

3. **Guarda el nombre de la base de datos:**
   ```bash
   dotnet user-secrets set "MongoDB:DatabaseName" "PropertyDB"
   ```

> **Ventaja:** Las credenciales se guardan localmente y no se suben a Git.

### Opción C: Usar Variables de Entorno

**Windows (PowerShell):**
```powershell
$env:ConnectionStrings__MongoDB = "mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority"
$env:MongoDB__DatabaseName = "PropertyDB"
```

**Linux/Mac:**
```bash
export ConnectionStrings__MongoDB="mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority"
export MongoDB__DatabaseName="PropertyDB"
```

---

## ✅ Paso 3: Verificar la Conexión

1. **Ejecuta la aplicación:**
   ```bash
   cd src\PropertyAPI
   dotnet run
   ```

2. **Verifica los logs:**
   - Deberías ver mensajes como:
     ```
     ✅ Cliente MongoDB creado exitosamente
     Cluster State: Connected
     ```

3. **Si hay errores**, revisa:
   - ✅ La cadena de conexión está correcta
   - ✅ Las credenciales son válidas
   - ✅ Tu IP está permitida en MongoDB Atlas (si aplica)

---

## 🌐 Paso 4: Acceder a MongoDB Atlas para Revisar Datos

### Método 1: Desde el Navegador (MongoDB Atlas Web UI)

1. **Inicia sesión en MongoDB Atlas:**
   - Ve a: https://cloud.mongodb.com/
   - Inicia sesión con las credenciales proporcionadas

2. **Navega a tu cluster:**
   - Selecciona el proyecto
   - Haz clic en el cluster `terranovamillion`

3. **Explora las colecciones:**
   - Haz clic en **"Browse Collections"**
   - Selecciona la base de datos `PropertyDB`
   - Verás las 4 colecciones:
     - `Properties`
     - `Owners`
     - `PropertyImages`
     - `PropertyTraces`

4. **Revisa los datos:**
   - Haz clic en cada colección para ver los documentos
   - Puedes filtrar, ordenar y buscar documentos
   - Puedes ver el esquema de cada documento

### Método 2: Usando MongoDB Compass

1. **Descarga MongoDB Compass:**
   - Ve a: https://www.mongodb.com/try/download/compass
   - Descarga e instala MongoDB Compass

2. **Conéctate usando la cadena de conexión:**
   - Abre MongoDB Compass
   - Pega la cadena de conexión completa
   - Haz clic en "Connect"

3. **Navega a la base de datos:**
   - Selecciona `PropertyDB`
   - Explora las colecciones y documentos

### Método 3: Usando mongosh (CLI)

1. **Instala MongoDB Shell:**
   - Descarga desde: https://www.mongodb.com/try/download/shell

2. **Conéctate:**
   ```bash
   mongosh "mongodb+srv://usuario:password@terranovamillion.8nyyhse.mongodb.net/PropertyDB?retryWrites=true&w=majority"
   ```

3. **Explora los datos:**
   ```javascript
   // Ver bases de datos
   show dbs
   
   // Usar la base de datos
   use PropertyDB
   
   // Ver colecciones
   show collections
   
   // Ver propiedades (primeros 5)
   db.Properties.find().limit(5).pretty()
   
   // Contar propiedades
   db.Properties.countDocuments()
   
   // Ver propietarios
   db.Owners.find().pretty()
   
   // Ver imágenes
   db.PropertyImages.find().limit(10).pretty()
   
   // Ver trazas
   db.PropertyTraces.find().limit(10).pretty()
   ```

---

## 🔍 Consultas Útiles en MongoDB Atlas

### Desde MongoDB Atlas Web UI

1. **Filtrar documentos:**
   - En la vista de colección, usa el filtro JSON
   - Ejemplo: `{ "address": { "$regex": "Bogotá", "$options": "i" } }`

2. **Buscar por campo:**
   - Usa la barra de búsqueda para buscar texto en todos los campos

3. **Ordenar:**
   - Haz clic en cualquier encabezado de columna para ordenar

### Desde mongosh

```javascript
// Buscar propiedades en Bogotá
db.Properties.find({ "address": /Bogotá/i }).pretty()

// Buscar propiedades por rango de precio
db.Properties.find({ 
  "price": { 
    "$gte": 200000000, 
    "$lte": 1000000000 
  } 
}).pretty()

// Buscar por código interno
db.Properties.find({ "codeInternal": "BOG-001" }).pretty()

// Ver propiedades con su propietario (usando lookup)
db.Properties.aggregate([
  {
    $lookup: {
      from: "Owners",
      localField: "idOwner",
      foreignField: "idOwner",
      as: "owner"
    }
  },
  { $limit: 5 }
]).pretty()

// Contar propiedades por ciudad
db.Properties.aggregate([
  {
    $group: {
      _id: { $substr: ["$codeInternal", 0, 3] },
      count: { $sum: 1 }
    }
  }
]).pretty()
```

---

## 🔒 Seguridad y Acceso

### Permisos de Acceso

- **Solo lectura:** Puedes revisar los datos sin modificarlos
- **Lectura y escritura:** Puedes modificar datos (depende de los permisos del usuario)

### Configuración de IP

Si no puedes conectarte, puede ser que tu IP no esté en la lista blanca:

1. **Contacta al administrador** para agregar tu IP
2. **O verifica en MongoDB Atlas:**
   - Ve a "Network Access"
   - Verifica si tu IP está en la lista
   - Si tienes permisos, puedes agregar tu IP actual

---

## 📊 Estructura de Datos

### Colección: Properties

```json
{
  "_id": "ObjectId",
  "Id": "string",
  "IdOwner": "owner-001",
  "Name": "Casa moderna en Chapinero",
  "Address": "Carrera 7 #56-12, Bogotá, Colombia",
  "Price": 920000000,
  "CodeInternal": "BOG-001",
  "Year": 2018,
  "CreatedAt": "2024-01-01T00:00:00Z",
  "UpdatedAt": "2024-01-01T00:00:00Z"
}
```

### Colección: Owners

```json
{
  "_id": "ObjectId",
  "IdOwner": "owner-001",
  "Name": "Laura Martínez",
  "Address": "Carrera 13 #54-80, Bogotá, Colombia",
  "Photo": "https://randomuser.me/api/portraits/women/45.jpg",
  "Birthday": "1982-03-05T00:00:00Z"
}
```

### Colección: PropertyImages

```json
{
  "_id": "ObjectId",
  "IdPropertyImage": "string",
  "IdProperty": "string",
  "File": "https://images.unsplash.com/...",
  "Enabled": true
}
```

### Colección: PropertyTraces

```json
{
  "_id": "ObjectId",
  "IdPropertyTrace": "string",
  "IdProperty": "string",
  "DateSale": "2024-01-01T00:00:00Z",
  "Name": "Registro de compra inicial",
  "Value": 920000000,
  "Tax": 73600000
}
```

---

## 🆘 Solución de Problemas

### Error: "Authentication failed"

- **Causa:** Usuario o contraseña incorrectos
- **Solución:** Verifica las credenciales proporcionadas
- **Nota:** Si tu contraseña tiene caracteres especiales, puede que necesites codificarla en URL

### Error: "IP not whitelisted"

- **Causa:** Tu IP no está en la lista de acceso
- **Solución:** 
  1. Contacta al administrador para agregar tu IP
  2. O si tienes acceso, ve a "Network Access" en MongoDB Atlas y agrega tu IP

### Error: "Connection timeout"

- **Causa:** Problemas de red o firewall bloqueando la conexión
- **Solución:** 
  1. Verifica tu conexión a internet
  2. Verifica que el firewall no esté bloqueando la conexión
  3. Intenta desde otra red

### No puedo ver las colecciones

- **Causa:** Puede que no tengas permisos o la base de datos esté vacía
- **Solución:**
  1. Verifica que estés conectado a la base de datos correcta (`PropertyDB`)
  2. Ejecuta el endpoint de seed: `POST /api/seed` para poblar datos
  3. Contacta al administrador si persiste el problema

---

## 📝 Comandos Rápidos

### Verificar conexión desde la aplicación

```bash
cd src\PropertyAPI
dotnet run
```

Si la aplicación inicia sin errores, la conexión está funcionando.

### Poblar datos de prueba

```bash
# Desde PowerShell
Invoke-RestMethod -Uri "http://localhost:5000/api/seed" -Method Post

# O desde cURL
curl -X POST http://localhost:5000/api/seed
```

### Agregar más datos

```bash
# Agregar 50 propiedades más
Invoke-RestMethod -Uri "http://localhost:5000/api/seed/add-more?count=50" -Method Post
```

---

## 💡 Tips

1. **MongoDB Atlas Web UI** es la forma más fácil de explorar los datos
2. **MongoDB Compass** es útil para análisis más avanzados
3. **mongosh** es ideal para scripts y automatización
4. **User Secrets** es la mejor opción para guardar credenciales localmente
5. **Variables de entorno** son ideales para producción

---

## 🔗 Recursos

- **MongoDB Atlas Dashboard**: https://cloud.mongodb.com/
- **MongoDB Compass**: https://www.mongodb.com/try/download/compass
- **MongoDB Shell (mongosh)**: https://www.mongodb.com/try/download/shell
- **Documentación MongoDB**: https://docs.mongodb.com/

---

**¡Listo!** Ahora puedes conectarte y revisar la base de datos en MongoDB Atlas. 🎉

