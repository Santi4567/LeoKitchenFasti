# 🦁🍳 LeoKitchenFasti

```text
=============================================================================
   _      ______ ____    _  _______ _______ _____ _    _ ______ _   _ 
  | |    |  ____/ __ \  | |/ /_   _|__   __/ ____| |  | |  ____| \ | |
  | |    | |__ | |  | | | ' /  | |    | | | |    | |__| | |__  |  \| |
  | |    |  __|| |  | | |  <   | |    | | | |    |  __  |  __| | . ` |
  | |____| |___| |__| | | . \ _| |_   | | | |____| |  | | |____| |\  |
  |______|______\____/  |_|\_\_____|  |_|  \_____|_|  |_|______|_| \_|

                       ______       _____ _______ _____ 
                      |  ____/\    / ____|__   __|_   _|
                      | |__ /  \  | (___    | |    | |  
                      |  __/ /\ \  \___ \   | |    | |  
                      | | / ____ \ ____) |  | |   _| |_ 
                      |_|/_/    \_\_____/   |_|  |_____|
                                                        
-----------------------------------------------------------------------------
     >> Sistema de Comandas y KDS en Tiempo Real - LeoKitchenFasti <<
     >> s4lm0.exe <<
=============================================================================
```

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

**LeoKitchenFasti** es un sistema de Punto de Venta (POS) móvil y Kitchen Display System (KDS) diseñado específicamente para eliminar el uso de papel en los restaurantes y optimizar la comunicación en tiempo real entre los meseros y la cocina.

---

## 🎯 El Problema

En muchos restaurantes de flujo rápido o con gran volumen (a la carta, buffets, asadores), el mesero actúa como un simple "mensajero". Toma la orden en papel, camina hasta la cocina, "canta" los platillos, y luego da múltiples viajes solo para revisar si la comida está lista. Esto genera:
*   Comandas perdidas o ilegibles.
*   Errores en las especificaciones del cliente.
*   Meseros perdiendo tiempo en la cocina en lugar de atender el piso.
*   Falta de sincronización en los tiempos de entrega.

## 🚀 Objetivos del Proyecto (El MVP)

**LeoKitchenFasti** nace con una filosofía estricta: **Ser una herramienta puramente operativa, no un ERP complejo.** 

Nuestros objetivos principales son:
1.  **Cero Papel:** Reemplazar las libretas por dispositivos móviles y las impresoras de tickets por pantallas táctiles (KDS) en cocina.
2.  **Sincronización en Tiempo Real:** Usar WebSockets (`SignalR`) para que la cocina vea la orden en el milisegundo en que el mesero la envía.
3.  **Flujo Inverso de Notificaciones:** Que la cocina notifique al dispositivo del mesero exactamente cuando un platillo está listo para ser recogido.
4.  **Simplicidad por Diseño:** Omitir a propósito módulos pesados como inventarios al gramo, nóminas o contabilidad fiscal. Nos enfocamos 100% en el servicio al cliente y la velocidad de entrega.

---

## 🍔 Casos de Uso Soportados

El sistema está diseñado para adaptarse a la caótica realidad de un restaurante mediante flujos de estado simples:

*   🍽️ **A la carta (Happy Path):** El mesero abre una mesa, toma la orden, la cocina recibe y prepara, notifica al mesero, se entrega y se cobra.
*   🔀 **Cambios Dinámicos:** Soporte para clientes que se cambian de mesa o reasignación de mesas si un mesero termina su turno.
*   🥩 **Estaciones Asíncronas (Asados/Parrilla):** El cliente elige su corte en un mostrador, el parrillero ingresa el peso y lo asocia a la mesa del cliente. El sistema notifica al mesero cuando está listo para llevarlo a la mesa.
*   🥗 **Modelo Buffet:** Ingreso rápido de "Covers" (personas) sin saturar la cocina, utilizando el sistema solo para auditar el cobro y pedir bebidas a la barra.

---

## 🏗️ Arquitectura y Tecnologías

El backend está construido para ser rápido, ligero y altamente reactivo.

*   **Lenguaje / Framework:** C# con ASP.NET Core.
*   **Comunicación en Tiempo Real:** SignalR para emitir eventos (`NuevaComanda`, `PlatilloListo`, `MesaTransferida`).
*   **Base de Datos:** Entity Framework Core con un diseño minimalista de 5 entidades principales:
    *   `Users` (Autenticación por PIN y Roles).
    *   `Tables` (Control de estados: Libre/Ocupada).
    *   `Products` (Catálogo simple).
    *   `Orders` (Cabecera de la cuenta).
    *   `OrderItems` (Detalle individual con control de estados: *Pendiente, EnPreparación, Listo*).
*   **Autenticación:** JWT (Access Token y Refresh Token) con políticas basadas en roles (Admin, Mesero, Cocina).

---

## 🚦 Primeros Pasos (Próximamente)

*(Aquí se agregarán las instrucciones para clonar el repositorio, configurar la cadena de conexión de la base de datos y ejecutar el servidor Kestrel).*


---
*Diseñado para hacer que la cocina fluya.* 🦁🔥