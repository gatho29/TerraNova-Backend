# 📚 Documentación de la API - TerraNova API

API RESTful para gestionar y consultar propiedades inmobiliarias con filtros avanzados.

---

## 🎯 Información General

### Base URL

**Desarrollo:**
```
http://localhost:5000
```

> **Nota**: En desarrollo, HTTPS redirection está deshabilitada para evitar problemas con CORS. Solo se usa HTTP.

**Producción:**
```
https://api.tudominio.com
```

### Formato de Datos

- **Content-Type**: `application/json`
- **Accept**: `application/json`
- Todas las respuestas están en formato JSON

---

## 📦 Modelos de Datos

### PropertyDto

Representa una propiedad inmobiliaria con todas sus relaciones.

```json
{
  "id": "string",
  "idOwner": "string",
  "name": "string",
  "address": "string",
  "price": 0.00,
  "codeInternal": "string",
  "year": 0,
  "owner": { /* OwnerDto */ },
  "images": [ /* PropertyImageDto[] */ ],
  "traces": [ /* PropertyTraceDto[] */ ]
}
```

#### Propiedades

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `id` | string | Sí | Identificador único de la propiedad |
| `idOwner` | string | Sí | ID del propietario |
| `name` | string | Sí | Nombre de la propiedad |
| `address` | string | Sí | Dirección completa |
| `price` | decimal | Sí | Precio de la propiedad |
| `codeInternal` | string | Sí | Código interno único de la propiedad |
| `year` | int | Sí | Año de construcción |
| `owner` | OwnerDto? | No | Información del propietario (cargado automáticamente) |
| `images` | PropertyImageDto[] | No | Lista de imágenes de la propiedad |
| `traces` | PropertyTraceDto[] | No | Historial de transacciones de la propiedad |

### OwnerDto

Representa al propietario de una propiedad.

```json
{
  "idOwner": "string",
  "name": "string",
  "address": "string",
  "photo": "string",
  "birthday": "2024-01-01T00:00:00Z"
}
```

### PropertyImageDto

Representa una imagen asociada a una propiedad.

```json
{
  "idPropertyImage": "string",
  "idProperty": "string",
  "file": "string",
  "enabled": true
}
```

### PropertyTraceDto

Representa el historial de transacciones de una propiedad.

```json
{
  "idPropertyTrace": "string",
  "idProperty": "string",
  "dateSale": "2024-01-01T00:00:00Z",
  "name": "string",
  "value": 0.00,
  "tax": 0.00
}
```

#### Ejemplo Completo

```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "idOwner": "owner-001",
  "name": "Casa moderna en Chapinero",
  "address": "Carrera 7 #56-12, Bogotá, Colombia",
  "price": 920000000.00,
  "codeInternal": "BOG-001",
  "year": 2018,
  "owner": {
    "idOwner": "owner-001",
    "name": "Laura Martínez",
    "address": "Carrera 13 #54-80, Bogotá, Colombia",
    "photo": "https://randomuser.me/api/portraits/women/45.jpg",
    "birthday": "1982-03-05T00:00:00Z"
  },
  "images": [
    {
      "idPropertyImage": "img-001",
      "idProperty": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "file": "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800",
      "enabled": true
    }
  ],
  "traces": [
    {
      "idPropertyTrace": "trace-001",
      "idProperty": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "dateSale": "2021-01-15T00:00:00Z",
      "name": "Registro de compra inicial",
      "value": 920000000.00,
      "tax": 73600000.00
    }
  ]
}
```

---

## 🔌 Endpoints

### 1. Obtener Todas las Propiedades

Obtiene una lista de propiedades con filtros opcionales.

#### Request

```http
GET /api/properties
```

#### Parámetros de Query (Opcionales)

| Parámetro | Tipo | Descripción | Ejemplo |
|-----------|------|-------------|---------|
| `name` | string | Búsqueda parcial por nombre (case-insensitive, sin acentos) | `medellin` (encuentra "Medellín") |
| `address` | string | Búsqueda parcial por dirección (case-insensitive, sin acentos) | `bogota` (encuentra "Bogotá") |
| `minPrice` | decimal | Precio mínimo | `200000000` |
| `maxPrice` | decimal | Precio máximo | `1000000000` |

#### Ejemplos de Request

**Sin filtros:**
```http
GET /api/properties
```

**Con filtro por nombre:**
```http
GET /api/properties?name=Madrid
```

**Con filtro por rango de precio:**
```http
GET /api/properties?minPrice=200000&maxPrice=300000
```

**Con múltiples filtros:**
```http
GET /api/properties?name=Casa&address=bogota&minPrice=200000000&maxPrice=1000000000
```

**Búsqueda sin acentos:**
```http
GET /api/properties?address=medellin
```
Esto encontrará propiedades con "Medellín", "MEDELLÍN", "medellín", etc.

#### Response

**Status Code: 200 OK**

```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "idOwner": "owner-001",
    "name": "Casa moderna en Chapinero",
    "address": "Carrera 7 #56-12, Bogotá, Colombia",
    "price": 920000000.00,
    "codeInternal": "BOG-001",
    "year": 2018,
    "owner": { /* OwnerDto */ },
    "images": [ /* PropertyImageDto[] */ ],
    "traces": [ /* PropertyTraceDto[] */ ]
  }
]
```

**Status Code: 400 Bad Request**

```json
{
  "error": "El precio mínimo no puede ser mayor que el precio máximo"
}
```

**Status Code: 500 Internal Server Error**

```json
{
  "error": "Ocurrió un error al procesar la solicitud"
}
```

---

### 2. Obtener Propiedad por ID

Obtiene una propiedad específica por su identificador único.

#### Request

```http
GET /api/properties/{id}
```

#### Parámetros de Ruta

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| `id` | string | Sí | ID de la propiedad |

#### Ejemplo de Request

```http
GET /api/properties/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

#### Response

**Status Code: 200 OK**

```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "idOwner": "owner-001",
  "name": "Casa moderna en Chapinero",
  "address": "Carrera 7 #56-12, Bogotá, Colombia",
  "price": 920000000.00,
  "codeInternal": "BOG-001",
  "year": 2018,
  "owner": { /* OwnerDto completo */ },
  "images": [ /* Array de PropertyImageDto */ ],
  "traces": [ /* Array de PropertyTraceDto */ ]
}
```

**Status Code: 404 Not Found**

```json
{
  "error": "No se encontró una propiedad con el ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

**Status Code: 400 Bad Request**

```json
{
  "error": "El ID no puede estar vacío"
}
```

---

### 3. Poblar Base de Datos (Seed)

Pobla la base de datos con datos de ejemplo. Útil para desarrollo y pruebas.

> **⚠️ Advertencia**: Este endpoint está diseñado para desarrollo. En producción, debería estar deshabilitado o protegido.

#### Request

```http
POST /api/seed
POST /api/seed?repopulate=true
```

#### Parámetros de Query

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `repopulate` | boolean | Si es `true`, elimina todos los datos existentes antes de poblar |

#### Ejemplo de Request

**Poblar (solo si está vacía):**
```http
POST /api/seed
Content-Type: application/json
```

**Repoblar (elimina y vuelve a poblar):**
```http
POST /api/seed?repopulate=true
Content-Type: application/json
```

#### Response

**Status Code: 200 OK**

```json
{
  "message": "Base de datos poblada exitosamente con datos de ejemplo",
  "summary": {
    "properties": 9,
    "owners": 9,
    "images": 15,
    "traces": 18
  }
}
```

> **Nota**: Si la base de datos ya contiene datos y no usas `repopulate=true`, el seed no se ejecutará.

---

### 4. Agregar Más Datos (Add More)

Agrega más propiedades a la base de datos sin eliminar las existentes.

> **⚠️ Advertencia**: Este endpoint está diseñado para desarrollo.

#### Request

```http
POST /api/seed/add-more?count=100
```

#### Parámetros de Query

| Parámetro | Tipo | Descripción | Default |
|-----------|------|-------------|---------|
| `count` | int | Número de propiedades a agregar | `100` |

#### Ejemplo de Request

```http
POST /api/seed/add-more?count=50
Content-Type: application/json
```

#### Response

**Status Code: 200 OK**

```json
{
  "message": "Se agregaron 50 propiedades adicionales exitosamente",
  "summary": {
    "properties": 59,
    "owners": 35,
    "images": 120,
    "traces": 118
  }
}
```

**Status Code: 500 Internal Server Error**

```json
{
  "error": "Ocurrió un error al agregar más datos",
  "details": "Mensaje de error detallado"
}
```

---

## 📊 Códigos de Estado HTTP

| Código | Descripción | Cuándo se usa |
|--------|-------------|---------------|
| `200 OK` | Solicitud exitosa | Operación completada correctamente |
| `400 Bad Request` | Solicitud inválida | Parámetros incorrectos o validación fallida |
| `404 Not Found` | Recurso no encontrado | Propiedad no existe |
| `500 Internal Server Error` | Error del servidor | Errores inesperados |

---

## 💡 Ejemplos de Uso

### cURL

#### Obtener todas las propiedades

```bash
curl -X GET "http://localhost:5000/api/properties" \
  -H "Accept: application/json"
```

#### Filtrar por nombre (sin acentos)

```bash
curl -X GET "http://localhost:5000/api/properties?name=medellin" \
  -H "Accept: application/json"
```

#### Filtrar por dirección (sin acentos)

```bash
curl -X GET "http://localhost:5000/api/properties?address=bogota" \
  -H "Accept: application/json"
```

#### Filtrar por rango de precio

```bash
curl -X GET "http://localhost:5000/api/properties?minPrice=200000000&maxPrice=1000000000" \
  -H "Accept: application/json"
```

#### Obtener propiedad por ID

```bash
curl -X GET "http://localhost:5000/api/properties/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Accept: application/json"
```

#### Poblar base de datos

```bash
# Poblar (solo si está vacía)
curl -X POST "http://localhost:5000/api/seed" \
  -H "Content-Type: application/json"

# Repoblar (elimina y vuelve a poblar)
curl -X POST "http://localhost:5000/api/seed?repopulate=true" \
  -H "Content-Type: application/json"

# Agregar más propiedades
curl -X POST "http://localhost:5000/api/seed/add-more?count=50" \
  -H "Content-Type: application/json"
```

---

### PowerShell

#### Obtener todas las propiedades

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/properties" -Method Get
```

#### Filtrar por nombre (sin acentos)

```powershell
$uri = "http://localhost:5000/api/properties?name=medellin"
Invoke-RestMethod -Uri $uri -Method Get
```

#### Filtrar por dirección (sin acentos)

```powershell
$uri = "http://localhost:5000/api/properties?address=bogota"
Invoke-RestMethod -Uri $uri -Method Get
```

#### Filtrar por rango de precio

```powershell
$uri = "http://localhost:5000/api/properties?minPrice=200000000&maxPrice=1000000000"
Invoke-RestMethod -Uri $uri -Method Get
```

#### Obtener propiedad por ID

```powershell
$id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
Invoke-RestMethod -Uri "http://localhost:5000/api/properties/$id" -Method Get
```

#### Poblar base de datos

```powershell
# Poblar (solo si está vacía)
Invoke-RestMethod -Uri "http://localhost:5000/api/seed" -Method Post

# Repoblar
Invoke-RestMethod -Uri "http://localhost:5000/api/seed?repopulate=true" -Method Post

# Agregar más propiedades
Invoke-RestMethod -Uri "http://localhost:5000/api/seed/add-more?count=50" -Method Post
```

---

### JavaScript (Fetch API)

#### Obtener todas las propiedades

```javascript
fetch('http://localhost:5000/api/properties')
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Error:', error));
```

#### Filtrar por nombre (sin acentos)

```javascript
const params = new URLSearchParams({ name: 'medellin' });
fetch(`http://localhost:5000/api/properties?${params}`)
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Error:', error));
```

#### Filtrar por dirección (sin acentos)

```javascript
const params = new URLSearchParams({ address: 'bogota' });
fetch(`http://localhost:5000/api/properties?${params}`)
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Error:', error));
```

#### Filtrar por rango de precio

```javascript
const params = new URLSearchParams({
  minPrice: '200000000',
  maxPrice: '1000000000'
});
fetch(`http://localhost:5000/api/properties?${params}`)
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Error:', error));
```

#### Obtener propiedad por ID

```javascript
const id = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';
fetch(`http://localhost:5000/api/properties/${id}`)
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Error:', error));
```


## 🔍 Swagger UI

La API incluye documentación interactiva con Swagger UI.

### Acceso

1. Inicia la aplicación
2. Navega a: `http://localhost:5000`
3. Verás la interfaz de Swagger con todos los endpoints
4. Puedes probar los endpoints directamente desde el navegador

> **Nota**: En desarrollo solo se usa HTTP. HTTPS está deshabilitado para evitar problemas con CORS.

### Características

- ✅ Documentación interactiva
- ✅ Pruebas de endpoints en tiempo real
- ✅ Esquemas de datos
- ✅ Ejemplos de requests y responses
- ✅ Validación de parámetros

---

## 🔍 Búsqueda Sin Acentos

La API implementa búsquedas inteligentes que ignoran acentos y mayúsculas/minúsculas. Esto significa que puedes buscar:

- **"medellin"** → encontrará "Medellín", "MEDELLÍN", "medellín", etc.
- **"bogota"** → encontrará "Bogotá", "BOGOTÁ", "bogotá", etc.
- **"cali"** → encontrará "Cali", "CALI", etc.

### Cómo Funciona

1. El término de búsqueda se normaliza (elimina acentos, convierte a minúsculas)
2. Se crea un patrón de expresión regular que incluye variantes con acentos
3. MongoDB busca usando este patrón con opción case-insensitive

### Ejemplos

```bash
# Buscar "medellin" (sin acento)
curl "http://localhost:5000/api/properties?address=medellin"
# Encontrará propiedades con "Medellín" en la dirección

# Buscar "BOGOTA" (mayúsculas, sin acento)
curl "http://localhost:5000/api/properties?address=BOGOTA"
# Encontrará propiedades con "Bogotá" en la dirección
```

---

## 🔍 Búsqueda Sin Acentos

La API implementa búsquedas inteligentes que ignoran acentos y mayúsculas/minúsculas. Esto significa que puedes buscar:

- **"medellin"** → encontrará "Medellín", "MEDELLÍN", "medellín", etc.
- **"bogota"** → encontrará "Bogotá", "BOGOTÁ", "bogotá", etc.
- **"cali"** → encontrará "Cali", "CALI", etc.

### Cómo Funciona

1. El término de búsqueda se normaliza (elimina acentos, convierte a minúsculas)
2. Se crea un patrón de expresión regular que incluye variantes con acentos
3. MongoDB busca usando este patrón con opción case-insensitive

### Ejemplos Prácticos

```bash
# Buscar "medellin" (sin acento) - encontrará "Medellín"
curl "http://localhost:5000/api/properties?address=medellin"

# Buscar "BOGOTA" (mayúsculas, sin acento) - encontrará "Bogotá"
curl "http://localhost:5000/api/properties?address=BOGOTA"

# Buscar "bogota" (minúsculas, sin acento) - encontrará "Bogotá"
curl "http://localhost:5000/api/properties?address=bogota"
```

### Caracteres Soportados

La normalización maneja los siguientes caracteres con acentos:

- **a**: a, á, à, ä, â, ã
- **e**: e, é, è, ë, ê
- **i**: i, í, ì, ï, î
- **o**: o, ó, ò, ö, ô, õ
- **u**: u, ú, ù, ü, û
- **n**: n, ñ
- **c**: c, ç

---

## ⚠️ Errores Comunes

### Error: "Access to fetch blocked by CORS policy"

**Causa**: El frontend no puede acceder al backend debido a políticas CORS.

**Solución**: 
1. Verifica que el backend esté corriendo
2. Verifica que CORS esté configurado en `Program.cs`
3. En desarrollo, asegúrate de que HTTPS redirection esté deshabilitada
4. Verifica que el frontend esté haciendo peticiones a `http://localhost:5000` (no HTTPS)

### Error: "Redirect is not allowed for a preflight request"

**Causa**: El servidor está redirigiendo peticiones HTTP a HTTPS, lo que rompe las peticiones preflight de CORS.

**Solución**: 
1. Asegúrate de que en desarrollo, HTTPS redirection esté deshabilitada
2. Verifica `Program.cs` - debe tener: `if (!app.Environment.IsDevelopment()) { app.UseHttpsRedirection(); }`
3. Usa solo HTTP en desarrollo: `http://localhost:5000`

### Error: "MongoDB connection string is not configured"

**Causa**: La cadena de conexión de MongoDB no está configurada.

**Solución**: Verifica el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "PropertyDB"
  }
}
```

### Error: "Cannot connect to MongoDB"

**Causa**: MongoDB no está corriendo o la cadena de conexión es incorrecta.

**Solución**:
1. Verifica que MongoDB esté corriendo (si es local)
2. Verifica el puerto (por defecto 27017 para local)
3. Si usas MongoDB Atlas, verifica la cadena de conexión
   - Consulta la [Guía de Conexión a MongoDB Atlas](./CONFIGURAR_MONGODB_ATLAS.md) para más detalles

### Error: "El precio mínimo no puede ser mayor que el precio máximo"

**Causa**: Valores inválidos para `minPrice` y `maxPrice`.

**Solución**: Asegúrate de que `minPrice` ≤ `maxPrice`.

### Error: 404 Not Found

**Causa**: El ID de la propiedad no existe.

**Solución**: Verifica que el ID sea correcto y que la propiedad exista.

### Error: 500 Internal Server Error

**Causa**: Error inesperado en el servidor.

**Solución**: 
1. Revisa los logs de la aplicación
2. Verifica la conexión a la base de datos
3. Contacta al administrador del sistema

---

## 📝 Notas Adicionales

### Filtros

- **Búsqueda parcial**: Los filtros `name` y `address` realizan búsquedas parciales
- **Case-insensitive**: Las búsquedas no distinguen entre mayúsculas y minúsculas
- **Sin acentos**: Las búsquedas ignoran acentos. Ejemplo: "medellin" encuentra "Medellín"
- **Combinación**: Puedes combinar múltiples filtros en una sola consulta
- **Ordenamiento**: Los resultados se ordenan por nombre de forma ascendente

### Estructura de Base de Datos

La base de datos utiliza 4 colecciones principales:

1. **Properties**: Propiedades inmobiliarias
2. **Owners**: Propietarios de las propiedades
3. **PropertyImages**: Imágenes asociadas a propiedades
4. **PropertyTraces**: Historial de transacciones de propiedades

Las relaciones se cargan automáticamente cuando se consulta una propiedad.

### Rendimiento

- La API utiliza índices en MongoDB para optimizar las consultas
- Los índices están en: `name`, `address`, `price`, `idOwner`, `codeInternal` (único)
- Las consultas con filtros son eficientes gracias a los índices
- Las entidades relacionadas se cargan de forma eficiente mediante consultas agrupadas

### Límites

- No hay límites de paginación implementados actualmente
- Para grandes volúmenes de datos, considera implementar paginación

---

## 🔗 Recursos Adicionales

- **Swagger UI**: `http://localhost:5000` (cuando la API está corriendo)
- **Documentación de .NET**: https://docs.microsoft.com/dotnet
- **Documentación de MongoDB**: https://docs.mongodb.com
- **Documentación de Swagger**: https://swagger.io/docs
- **Conectar a MongoDB Atlas**: [CONFIGURAR_MONGODB_ATLAS.md](./CONFIGURAR_MONGODB_ATLAS.md)

---

## 📞 Soporte

Para soporte o preguntas sobre la API:

- **Email**: support@propertyapi.com
- **Documentación**: Consulta este documento o Swagger UI
- **Issues**: Reporta problemas en el repositorio del proyecto

---

**Versión de la API**: v1  
**Última actualización**: 2024

