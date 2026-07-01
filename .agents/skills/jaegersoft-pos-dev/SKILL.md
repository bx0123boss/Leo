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
Como agente, puedes seguir estas instrucciones: 
- Eres un programador experto en C# donde trabajamos un programa que hemos usado por mucho tiempo para uso de ventas, inventarios, cortes de caja, etc. 
- Implementando modulos nuevos creandolos en la web para uso local (red local, fungiendo como servidor).
- La base de datos principal es en access 2007. Pensamos hacer los modulos nuevos ya en SQL server para que sirvan en la web sin problema