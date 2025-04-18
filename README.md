
# 🧾 Aplicación de Gestión de Facturas – Backend (.NET 8 + EF Core + SQLite)

## 1. 📌 ¿Que realiza esta API?

Este backend expone una **API REST** que permite cargar, consultar y administrar facturas. Entre sus funcionalidades principales se encuentran:

- ✅ Subida de un archivo JSON con múltiples facturas
- ✅ Listado completo de facturas registradas
- ✅ Búsqueda de facturas por número, estado de factura y estado de pago
- ✅ Visualización del detalle de una factura específica
- ✅ Registro de Notas de Crédito asociadas a una factura
- ✅ Cálculo automático de:
  - Estado de la factura: `Issued`, `Partial`, `Cancelled`
  - Estado de pago: `Pending`, `Overdue`, `Paid`

Esta API es consumida por el frontend desarrollado en ReactJS.

---

## 2. ⚙️ Tecnologías utilizadas

- **.NET 8 (.NET Core Web API)**
- **Entity Framework Core**
- **SQLite** como base de datos embebida
- **LINQ** para consultas y relaciones
- **CORS** habilitado para frontend local en `http://localhost:5173`

---

## 3. ▶️ ¿Cómo ejecutar el proyecto?

### Requisitos

- .NET SDK 8 instalado  
  [Descargar desde el sitio oficial](https://dotnet.microsoft.com/download)

### Pasos para ejecutar

1. Abre el proyecto en Visual Studio

2. Restaura las dependencias: dotnet restore --> Este comando descarga e instala los paquetes NuGet (como Entity Framework Core, etc.) que necesita tu proyecto para funcionar.

⚠️ Si usas Visual Studio, esto se hace automaticamente al abrir y compilar el proyecto.

3. Ejecutar el proyecto: dotnet run  --> Este comando inicia tu API backend. Despues de ejecutarlo, tu API estara corriendo localmente

## 4. 🔌 Endpoints principales

- ** GET /Invoice/getAllInvoice ** --> Retorna el listado completo de todas las facturas cargadas.

- ** POST /Invoice/creditNotesByInvoice ** --> Devuelve todas las notas de crédito asociadas a una factura específica.

- ** POST /Invoice/uploadInvoice ** --> Carga un archivo JSON con facturas.

- ** POST /Invoice/search ** --> Permite buscar facturas filtrando por número, estado de factura y estado de pago.

- ** POST /Invoice/creditNote ** --> Agrega una nota de crédito a una factura específica.





