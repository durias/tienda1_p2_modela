using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimioAPI;
using SimioAPI.Extensions;
using SimioAPI.Graphics;
using Simio;
using Simio.SimioEnums;
using QlmLicenseLib;
using System.Drawing.Printing;

namespace _MYS1_API_P13
{
    class generador_objetos
    {
        private ISimioProject proyectoApi;
        public static string rutabase = "[MYS1]ModeloBase_P13.spfx";
        public static string rutafinal = "[MYS1]Tienda1_P13.spfx";
        private string[] warnings;
        private IModel model;
        private IIntelligentObjects intelligentObjects;

        public generador_objetos()
        {
            proyectoApi = SimioProjectFactory.LoadProject(rutabase, out warnings);
            model = proyectoApi.Models[1];
            intelligentObjects = model.Facility.IntelligentObjects;
        }

        public void crearModelo()
        {
            crear_modelo();
            SimioProjectFactory.SaveProject(proyectoApi, rutafinal, out warnings);
        }
        //Inicio de la creación del modelo 
        public void crear_modelo()
        {   //Creacion del area inicial del sistema
            crear_source_clientes();
            crear_source_almacen();
            crear_combiner_botellas();
            crear_server_caja();
            //Enlaces del area inicial
            crear_path_llegada_caja();
            crear_path_caja_entrega();
            crear_path_almacen_entrega();
            //Creacion del transfer node que dirige hacia las mesas 
            crear_transfer_selector_mesas();
            crear_path_entrega_selector_mesas();
            //Mesas 1-8
            crear_server_mesa1();
            crear_server_mesa2();
            crear_server_mesa3();
            crear_server_mesa4();
            crear_server_mesa5();
            crear_server_mesa6();
            crear_server_mesa7();
            crear_server_mesa8();
            crear_timepath_selector_mesas_1_8();
            //Mesas 9-11
            crear_server_mesa9();
            crear_server_mesa10();
            crear_server_mesa11();
            crear_timepath_selector_mesas_9_11();
            //Mesa 12
            crear_server_mesa12();
            crear_path_selector_mesa_12();
            //Salida           
            crear_salidas_desde_mesas();
        }

        //////CREACION DEL AREA DE INGRESO, CAJA Y ALMACEN------------------------------------------------

        /*
         * Como un cliente solo puede ser atendido hasta que el encargado termine de entregar las botellas
         * el cliente solo ingresa hasta que se termina de entregar un pedido, por eso se utiliza el evento
         * output@Entrega.Exited para originar las entidades cliente.
         */
        public void crear_source_clientes() {
            createSource("Clientes", -69, -9);
            set_propiedad("Clientes", "ArrivalMode", "On Event");
            set_propiedad("Clientes", "EntityType", "Cliente");
            set_propiedad("Clientes", "InitialNumberEntities", "1");
            set_propiedad("Clientes", "TriggeringEventName", "Output@Entrega.Exited");
            createEntity("Cliente", -72, -9);
        }

        public void crear_server_caja() {
            createServer("Caja", -44, -9);
            set_propiedad("Caja", "ProcessingTime", "Random.Uniform(0.42, 1.83)");
        }

        //Genera las botellas que son enviadas al combiner
        public void crear_source_almacen()
        {
            createSource("Almacen", -70, 4);
            set_propiedad("Almacen", "InterarrivalTime", "Random.Uniform(0.017,0.021)");
            set_propiedad("Almacen", "EntityType", "Botella");
            createEntity("Botella", -73, 4);
        }
        //En este combiner se reunen las entidades botella, generadas por el almacen
        public void crear_combiner_botellas() {
            createCombiner("Entrega", -41, 3);
            set_propiedad("Entrega", "BatchQuantity", "4");
        }
        //Los siguientes metodos son para enlazar los objetos del area inicial del sistema
        public void crear_path_llegada_caja() {

            createPath("Path_llegada_caja", get_multi_Nodo("Clientes", 0), get_multi_Nodo("Caja", 0));
            set_propiedad("Path_llegada_caja", "DrawnToScale", "False");
            set_propiedad("Path_llegada_caja", "LogicalLength", "10");
        }

        public void crear_path_caja_entrega()
        {
            createPath("Path_caja_entrega", get_multi_Nodo("Caja", 1), get_multi_Nodo("Entrega", 0));
            set_propiedad("Path_caja_entrega", "DrawnToScale", "False");
            set_propiedad("Path_caja_entrega", "LogicalLength", "5");
        }

        public void crear_path_almacen_entrega()
        {
            createPath("Path_almacen_entrega", get_multi_Nodo("Almacen", 0), get_multi_Nodo("Entrega", 1));
        }

        //Este transfer node es el que se encarga de dirigir las entidades hacia las mesas 
        public void crear_transfer_selector_mesas()
        {
            createTransferNode("selector_mesas", 10, 40);
            set_propiedad("selector_mesas", "OutBoundLinkRule", "By Link Weight");
        }

        public void crear_path_entrega_selector_mesas() {

            createPath("entrega_selector", get_multi_Nodo("Entrega", 2), get_nodo("selector_mesas"));
        }


        //CREACION DE LOS SERVERS QUE REPRESENTAN LAS MESAS //////////////////////////////////////////////

        /// Mesas de la 1 a la 8 

        public void crear_server_mesa1()
        {
            createServer("mesa_1", -40, 60);
            set_propiedad("mesa_1", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_1", "InitialCapacity", "4");
        }
        public void crear_server_mesa2()
        {
            createServer("mesa_2", -40, 40);
            set_propiedad("mesa_2", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_2", "InitialCapacity", "4");
        }
        public void crear_server_mesa3()
        {
            createServer("mesa_3", 0, 10);
            set_propiedad("mesa_3", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_3", "InitialCapacity", "4");
        }
        public void crear_server_mesa4()
        {
            createServer("mesa_4", 21, 10);
            set_propiedad("mesa_4", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_4", "InitialCapacity", "4");
        }
        public void crear_server_mesa5()
        {
            createServer("mesa_5", 60, 40);
            set_propiedad("mesa_5", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_5", "InitialCapacity", "4");
        }
        public void crear_server_mesa6()
        {
            createServer("mesa_6", 60, 60);
            set_propiedad("mesa_6", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_6", "InitialCapacity", "4");
        }
        public void crear_server_mesa7()
        {
            createServer("mesa_7", 33, 90);
            set_propiedad("mesa_7", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_7", "InitialCapacity", "4");
        }
        public void crear_server_mesa8()
        {
            createServer("mesa_8", -14, 90);
            set_propiedad("mesa_8", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_8", "InitialCapacity", "4");
        }
        //Mesas de la 9 a la 11 
        public void crear_server_mesa9()
        {
            createServer("mesa_9", -14, 60);
            set_propiedad("mesa_9", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_9", "InitialCapacity", "3");
        }
        public void crear_server_mesa10()
        {
            createServer("mesa_10", 31, 60);
            set_propiedad("mesa_10", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_10", "InitialCapacity", "3");
        }
        public void crear_server_mesa11()
        {
            createServer("mesa_11", 1, 71);
            set_propiedad("mesa_11", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_11", "InitialCapacity", "3");
        }
        //Mesa 12 
        public void crear_server_mesa12()
        {
            createServer("mesa_12", 12, 59);
            set_propiedad("mesa_12", "ProcessingTime", "Random.Triangular(10,20,30)");
            set_propiedad("mesa_12", "InitialCapacity", "8");
        }

        /// Creacion de los enlaces hacia las mesas 1 a la 8//////////////////////////////////////////////////////
        /*
         Como la probabilidad de que elija una mesa de la 1 a la 8 es de 0.3 se dividió 0.3 entre las
         8 posibles mesas que podrían elegirse
         El tiempo que tarda cada entidad en moverse hacia cualquiera de las opciones se ingresó en minutos
         */
        public void crear_timepath_selector_mesas_1_8() {
            //Mesa1
            createTimePath("tp_selector_mesa_1", get_nodo("selector_mesas"), get_multi_Nodo("mesa_1", 0));
            set_propiedad("tp_selector_mesa_1", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_1", "SelectionWeight", "0.0375");
            //Mesa2
            createTimePath("tp_selector_mesa_2", get_nodo("selector_mesas"), get_multi_Nodo("mesa_2", 0));
            set_propiedad("tp_selector_mesa_2", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_2", "SelectionWeight", "0.0375");
            //Mesa3
            createTimePath("tp_selector_mesa_3", get_nodo("selector_mesas"), get_multi_Nodo("mesa_3", 0));
            set_propiedad("tp_selector_mesa_3", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_3", "SelectionWeight", "0.0375");
            //Mesa4
            createTimePath("tp_selector_mesa_4", get_nodo("selector_mesas"), get_multi_Nodo("mesa_4", 0));
            set_propiedad("tp_selector_mesa_4", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_4", "SelectionWeight", "0.0375");
            //Mesa5
            createTimePath("tp_selector_mesa_5", get_nodo("selector_mesas"), get_multi_Nodo("mesa_5", 0));
            set_propiedad("tp_selector_mesa_5", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_5", "SelectionWeight", "0.0375");
            //Mesa6
            createTimePath("tp_selector_mesa_6", get_nodo("selector_mesas"), get_multi_Nodo("mesa_6", 0));
            set_propiedad("tp_selector_mesa_6", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_6", "SelectionWeight", "0.0375");
            //Mesa7
            createTimePath("tp_selector_mesa_7", get_nodo("selector_mesas"), get_multi_Nodo("mesa_7", 0));
            set_propiedad("tp_selector_mesa_7", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_7", "SelectionWeight", "0.0375");
            //Mesa8
            createTimePath("tp_selector_mesa_8", get_nodo("selector_mesas"), get_multi_Nodo("mesa_8", 0));
            set_propiedad("tp_selector_mesa_8", "TravelTime", "0.083");
            set_propiedad("tp_selector_mesa_8", "SelectionWeight", "0.0375");
        }
        /// Creacion de los enlaces hacia las mesas 9 a la 11 //////////////////////////////////////////////////////
        /*
         Para estos casos la probabilidad de que elija una mesa de la 9 a la 11 es de 0.12 se dividió 0.12 entre las
         8 posibles mesas que podrían elegirse
         El tiempo que tarda cada entidad en moverse hacia cualquiera de las opciones se ingresó en minutos
         */
        public void crear_timepath_selector_mesas_9_11()
        {
            //Mesa9
            createTimePath("tp_selector_mesa_9", get_nodo("selector_mesas"), get_multi_Nodo("mesa_9", 0));
            set_propiedad("tp_selector_mesa_9", "TravelTime", "0.1667");
            set_propiedad("tp_selector_mesa_9", "SelectionWeight", "0.04");
            //Mesa10
            createTimePath("tp_selector_mesa_10", get_nodo("selector_mesas"), get_multi_Nodo("mesa_10", 0));
            set_propiedad("tp_selector_mesa_10", "TravelTime", "0.1667");
            set_propiedad("tp_selector_mesa_10", "SelectionWeight", "0.04");
            //Mesa11
            createTimePath("tp_selector_mesa_11", get_nodo("selector_mesas"), get_multi_Nodo("mesa_11", 0));
            set_propiedad("tp_selector_mesa_11", "TravelTime", "0.1667");
            set_propiedad("tp_selector_mesa_11", "SelectionWeight", "0.04");
        }

        /// Creacion del enlace hacia la mesa 12 //////////////////////////////////////////////////////
        /*
         La probabilidad especificada es de 0.12 y la mesa se encuentra a 20 metros
       
         */
        public void crear_path_selector_mesa_12()
        {
            createPath("path_mesa_12", get_nodo("selector_mesas"), get_multi_Nodo("mesa_12", 0));
            set_propiedad("path_mesa_12", "DrawnToScale", "False");
            set_propiedad("path_mesa_12", "LogicalLength", "20");
            set_propiedad("path_mesa_12", "SelectionWeight", "0.06");

        }
       
        //Creacion de las salidas que simulan cuando el cliente se va luego de estar en la mesa un tiempo
        /*
         Se creó un sink para cada mesa, con el fin de darle una mejor presentación, pero basicamente
         cada sink representa la acción de cuando el cliente se va de una mesa y sale de la tienda.
         Cada salida tiene el tiempo especificado de 0.33 minutos que tarda el cliente en moverse
         de la mesa hasta la salida. Excepto la salida directa de la entrega, esta tiene un tiempo de 0.25 minutos.
         */
        public void crear_salidas_desde_mesas() {
            //Salida mesa 1
            createSink("salida_mesa_1", -37, 75);
            createTimePath("tp_mesa1_salida", get_multi_Nodo("mesa_1", 1), get_multi_Nodo("salida_mesa_1",0));
            set_propiedad("tp_mesa1_salida","TravelTime","0.33");
            //Salida mesa 2
            createSink("salida_mesa_2", -37, 50);
            createTimePath("tp_mesa2_salida", get_multi_Nodo("mesa_2", 1), get_multi_Nodo("salida_mesa_2", 0));
            set_propiedad("tp_mesa2_salida", "TravelTime", "0.33");
            //Salida mesa 3
            createSink("salida_mesa_3", 3, 0);
            createTimePath("tp_mesa3_salida", get_multi_Nodo("mesa_3", 1), get_multi_Nodo("salida_mesa_3", 0));
            set_propiedad("tp_mesa3_salida", "TravelTime", "0.33");
            //Salida mesa 4
            createSink("salida_mesa_4", 24, 0);
            createTimePath("tp_mesa4_salida", get_multi_Nodo("mesa_4", 1), get_multi_Nodo("salida_mesa_4", 0));
            set_propiedad("tp_mesa4_salida", "TravelTime", "0.33");
            //Salida mesa 5
            createSink("salida_mesa_5", 73, 40);
            createTimePath("tp_mesa5_salida", get_multi_Nodo("mesa_5", 1), get_multi_Nodo("salida_mesa_5", 0));
            set_propiedad("tp_mesa5_salida", "TravelTime", "0.33");
            //Salida mesa 6
            createSink("salida_mesa_6", 73, 60);
            createTimePath("tp_mesa6_salida", get_multi_Nodo("mesa_6", 1), get_multi_Nodo("salida_mesa_6", 0));
            set_propiedad("tp_mesa6_salida", "TravelTime", "0.33");
            //Salida mesa 7
            createSink("salida_mesa_7", 36, 106);
            createTimePath("tp_mesa7_salida", get_multi_Nodo("mesa_7", 1), get_multi_Nodo("salida_mesa_7", 0));
            set_propiedad("tp_mesa7_salida", "TravelTime", "0.33");
            //Salida mesa 8
            createSink("salida_mesa_8",-11 ,106);
            createTimePath("tp_mesa8_salida", get_multi_Nodo("mesa_8", 1), get_multi_Nodo("salida_mesa_8", 0));
            set_propiedad("tp_mesa8_salida", "TravelTime", "0.33");
            //Salida mesa 9
            createSink("salida_mesa_9",-11 ,70);
            createTimePath("tp_mesa9_salida", get_multi_Nodo("mesa_9", 1), get_multi_Nodo("salida_mesa_9", 0));
            set_propiedad("tp_mesa9_salida", "TravelTime", "0.33");
            //Salida mesa 10
            createSink("salida_mesa_10",34 ,70 );
            createTimePath("tp_mesa10_salida", get_multi_Nodo("mesa_10", 1), get_multi_Nodo("salida_mesa_10", 0));
            set_propiedad("tp_mesa10_salida", "TravelTime", "0.33");
            //Salida mesa 11
            createSink("salida_mesa_11",4 ,90 );
            createTimePath("tp_mesa11_salida", get_multi_Nodo("mesa_11", 1), get_multi_Nodo("salida_mesa_11", 0));
            set_propiedad("tp_mesa11_salida", "TravelTime", "0.33");
            //Salida mesa 12
            createSink("salida_mesa_12",15 ,74);
            createTimePath("tp_mesa12_salida", get_multi_Nodo("mesa_12", 1), get_multi_Nodo("salida_mesa_12", 0));
            set_propiedad("tp_mesa12_salida", "TravelTime", "0.33");
            //Salida directamente desde la entrega 
            createSink("salida_directa", 51, 20);
            createTimePath("tp_mesas_salida", get_nodo("selector_mesas"), get_multi_Nodo("salida_directa", 0));
            set_propiedad("tp_mesas_salida", "TravelTime", "0.25");
            set_propiedad("tp_mesas_salida", "SelectionWeight", "0.52");

        }


        public void createSource(string nombre, int x, int y)
        {
            intelligentObjects.CreateObject("Source", new FacilityLocation(x, 0, y));
            model.Facility.IntelligentObjects["Source1"].ObjectName = nombre;
        }

        public void createServer(string nombre, int x, int y)
        {
            intelligentObjects.CreateObject("Server", new FacilityLocation(x, 0, y));
            model.Facility.IntelligentObjects["Server1"].ObjectName = nombre;
        }

        public void set_propiedad(String name, String property, String value)
        {
            model.Facility.IntelligentObjects[name].Properties[property].Value = value;
        }

        public void createCombiner(string nombre, int x, int y)
        {
            intelligentObjects.CreateObject("Combiner", new FacilityLocation(x, 0, y));
            model.Facility.IntelligentObjects["Combiner1"].ObjectName = nombre;
        }

        public void createEntity(string nombre, int x, int y)
        {
            intelligentObjects.CreateObject("ModelEntity", new FacilityLocation(x, 0, y));
            model.Facility.IntelligentObjects["ModelEntity1"].ObjectName = nombre;        
        }

        public void createPath(string nombre, INodeObject nodo1, INodeObject nodo2)
        {
           this.createLink("Path", nodo1, nodo2);
           model.Facility.IntelligentObjects["Path1"].ObjectName = nombre;
        }

        //Devuelve uno de los nodos que se espeficican con el entero nodo
        public INodeObject get_multi_Nodo(String name, int nodo)
        {
            return ((IFixedObject)model.Facility.IntelligentObjects[name]).Nodes[nodo];
        }
        //Devuelve el nodo de un objeto que solamente tiene un enlace
        public INodeObject get_nodo(String name)
        {
            return (INodeObject)model.Facility.IntelligentObjects[name];
        }

        public void createTransferNode(string nombre, int x, int y)
        {
            intelligentObjects.CreateObject("TransferNode", new FacilityLocation(x, 0, y));
            model.Facility.IntelligentObjects["TransferNode1"].ObjectName = nombre;
        }

        public void createTimePath(string nombre,INodeObject nodo1, INodeObject nodo2)
        {
            this.createLink("TimePath", nodo1, nodo2);
            model.Facility.IntelligentObjects["TimePath1"].ObjectName = nombre;
        }
        public void createSink(string nombre, int x, int y)
        {
            intelligentObjects.CreateObject("Sink", new FacilityLocation(x, 0, y));
            model.Facility.IntelligentObjects["Sink1"].ObjectName = nombre;
        }

        
        public void createLink(String type, INodeObject nodo1, INodeObject nodo2)
        {
            intelligentObjects.CreateLink(type, nodo1, nodo2, null);
        }

    }
}
