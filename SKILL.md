---
name: jaegersoft-pos-dev
description: Instrucciones y contexto para desarrollar en el Punto de Venta (POS) Jaegersoft (Proyecto Leo).
---

# Jaegersoft POS (Proyecto Leo)

Esta "skill" contiene el contexto principal para trabajar en este repositorio. El proyecto es un sistema de Punto de Venta (POS) desarrollado en C# con Windows Forms.

## Reglas y Convenciones

1. **Estilos de UI**:
   - Usar la clase `frmBase` como plantilla para formularios nuevos cuando aplique.
   - Usar el método `EstilizarDataGridView()` en `frmBase` para el diseño oscuro de las tablas.
   - Usar `EstilizarBotonPrimario`, `EstilizarBotonPeligro`, etc., para mantener coherencia en los botones.

2. **Base de Datos**:
   - La aplicación parece usar OLEDB y SQL Server para la conectividad de base de datos.
   - Siempre usar consultas parametrizadas u objetos de comando para evitar inyecciones.
   - Asegurarse de cerrar y liberar (`using`) las conexiones y recursos de base de datos.

3. **Arquitectura**:
   - `BRUNO` contiene la mayoría de los formularios principales del sistema (Ventas, Inventario, Consignas, etc.).

*(Puedes editar este archivo y agregar todas las instrucciones específicas que desees que el agente recuerde de ahora en adelante al trabajar en este proyecto).*
