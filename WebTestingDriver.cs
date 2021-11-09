using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System.Collections.ObjectModel;
using System.Threading;

namespace MeuSeleniumCSharp
{
    public enum eTipoDriver : int
    {
        ChromeDriver = 0,
        EdgeDriver = 1
    }
    public enum eTipoElemento : int
    {
        Input = 0,
        Opcao = 10,
        Botao = 20
    }
    public class TestSuite
    {

        private TestHub Hub;

        private QA_WebMotor Motor;

        public eTipoDriver tipoDriver;

        public List<TestScript> Scripts = new List<TestScript>();
        public string nome
        {
            get => this.GetType().Name;
        }

        public TestConfig Config
        {
            get => Hub.Projeto.Config;
        }

        public void Setup(TestHub prmHub)
        {
            Hub = prmHub;
        }

        public void AddScript(TestScript prmScript)
        {
            Scripts.Add(prmScript);
        }

        public void Executar(eTipoDriver prmTipoDriver)
        {

            tipoDriver = prmTipoDriver;

            Motor = new QA_WebMotor(this);

            foreach (TestScript Script in Scripts)
            {
                Script.Executar(Motor);
            }

            Motor.Encerrar();

        }

    }
    public class TestScript
    {

        public string nome;

        private QA_WebMotor _motor;
        public string nome_extendido()
        { return (Suite.nome + '.' + nome); }

        public string metodo_extendido(string prmMetodo)
        { return (nome_extendido() + ':' + prmMetodo + "()"); }

        public void Executar(QA_WebMotor prmMotor)
        {

            this.nome = this.GetType().Name;
            this.Motor = prmMotor;

            Metodo("DATA") ;

            if (Dados.IsOK)
            {

                Metodo("SETUP");

                MetodoEXECUCAO();

            }

            Robot.Pause(Config.PauseAfterTestCase);

        }
        private void MetodoEXECUCAO()
        {

            if (Dados.IsOFF)
                Metodo("PLAY;CHECK;CLEANUP");
            else
            {
                while (Dados.Next())
                {
                    Metodo("PLAY;CHECK;CLEANUP");
                    Motor.Refresh();
                }
            }
        }

        private void Metodo(string prmNome)
        {

            xLista lista = new xLista(prmNome);

            foreach (string metodo in lista)
            {
                try
                {
                    this.GetType().GetMethod(metodo).Invoke(this, null);
                }

                catch
                {
                    Robot.Log.Aviso("SCRIPT: " + metodo_extendido(metodo) + " não encontrado");
                }

            }


        }

        public QA_WebMotor Motor
        {
            get => _motor;
            set => _motor = value;
        }
        public QA_WebRobot Robot
        {
            get => Motor.Robot;
        }
        public QA_DataPool Dados
        {
            get => Robot.Dados;
        }
        public TestSuite Suite
        {
            get => Motor.Suite;
        }
        public TestConfig Config
        {
            get => Suite.Config;
        }
    }
    public class QA_WebPage
    {

        public QA_WebRobot Robot;

        public QA_WebMapping Mapa;

        private IWebElement LastControl;

        public QA_WebPage(QA_WebRobot prmRobot)
        {

            Robot = prmRobot;

            Mapa = new QA_WebMapping(this);

        }

        public QA_WebElemento Elemento
        { get => Mapa.Elemento; }
        public QA_WebLog Log
        { get => Robot.Log; }
        public QA_WebElemento AddItem(string prmKey, string prmTarget)
        {
            return (Mapa.AddKey(prmKey, prmTarget));
        }
        public bool SetAction(string prmValor)
        {

            if (Elemento.SetAction(prmValor))
            {
                LastControl = Elemento.control;
                return (true);
            }
            return (false);
        }
        public bool Submit()
        {
            try
            {
                if (TemLastControl())
                {
                    LastControl.Submit();              
                    return (true);
                }
            }

            catch (Exception e)
            { Log.Erro(e.Message); }

            return (false);
        }

        public void Refresh()
        {  Mapa.Refresh(); }

        private bool TemLastControl()
        { return !(LastControl == null); }
    }
    public class QA_WebMapping
    {
        
        private QA_WebPage Page;

        public List<QA_WebElemento> Elementos;

        public QA_WebMapping(QA_WebPage prmPage)
        {

            Page = prmPage;

            Elementos = new List<QA_WebElemento> { };

        }

        private QA_WebElemento _elemento;

        public QA_WebElemento Elemento
        { get => _elemento; }

        public QA_WebElemento AddKey(string prmKey, string prmTarget)
        {

            QA_WebElemento elemento = new QA_WebElemento(Page, prmKey, prmTarget);

            Elementos.Add(elemento);

            return (elemento);
        }
        public bool FindKey(string prmKey)
        {

            _elemento = null;

            foreach (QA_WebElemento item in Elementos)
            {
                if (item.key == prmKey.ToLower())
                {

                    _elemento = item;

                    break;
                }
            }

            return (TemElemento());
        }
        public bool SetTarget(string prmTarget)
        {

            xTupla Target = new xTupla();

            Target.Set(prmTarget);

            return Elemento.SetAction(Target.valor);
        }
        public void Refresh()
        {

            _elemento = null;

            foreach (QA_WebElemento item in Elementos)
                item.Refresh();
        }

        public bool TemElemento()
        { return !(_elemento == null); }
    }
    public class QA_WebElemento
    {

        public QA_WebPage Page;

        private string _key;

        private eTipoElemento _tipo = eTipoElemento.Input;

        private xTupla _chave = new xTupla();

        private QA_WebDominio Dominio;

        private QA_WebTarget _target;

        public QA_WebElemento(QA_WebPage prmPage, string prmKey, string prmTarget)
        {
            Page = prmPage;

            _key = prmKey.ToLower();
            _tipo = eTipoElemento.Input;

            _chave.Set(prmTarget);

            Dominio = new QA_WebDominio(this);

        }

        public QA_WebTarget Target
        { get
            {
                if (_target == null)
                    { _target = new QA_WebTarget(this); }
                return _target;
            }
        }
        public QA_WebRobot Robot
        { get => Page.Robot; }
        public string key
        { get => _key; }
        public eTipoElemento tipo
        { get => _tipo; }
        public xTupla Chave
        { get => _chave; }

        public string filtro;
        public IWebElement control
        { get => Target.control; }
        public void SetDomain(string prmLista)
        {
            SetDomain(prmLista, null);
        }
        public void SetDomain(string prmLista, string prmSintaxe)
        {
            Dominio.Setup(prmLista, prmSintaxe);
        }
        public bool SetAction(string prmValor)
        {

            if (Dominio.IsON())
            {
                return (Dominio.SetAction(prmValor));
            }
            else
            {
                switch (tipo)
                {

                    case eTipoElemento.Input:
                        {
                            if (Click())
                            {
                                Clear();
                                return (SendKeys(prmValor));
                            }
                            return (false);
                        }

                    case eTipoElemento.Botao:
                        { return Click(); }

                }

            }

            return Erro(prmErro: "AÇÃO não encontrada ... " + tipo);

        }
        public bool Clear()
        {
            try
            { control.Clear(); return (true); }

            catch (Exception e)
            { Erro(e.Message); }

            return (false);
        }
        public bool Click()
        {
            try
            { control.Click(); return (true); }

            catch (Exception e)
            { Erro(e.Message); }

            return (false);
        }
        public bool SendKeys(string prmTexto)
        {
            try
            { control.SendKeys(prmTexto); return (true); }

            catch (Exception e)
            { Erro(e.Message); }

            return (false);
        }
        public void Refresh()
        { _target = null; }
        public bool Erro(string prmErro)
        { return Page.Log.Erro("QA-LOG: " + prmErro); }
    }
    public class QA_WebDominio
    {
        private QA_WebElemento Elemento;

        private xLista lista;

        private string filtro;

        private ReadOnlyCollection<IWebElement> Opcoes;
        public xTupla Chave
        { get => Elemento.Chave; }
        public QA_WebRobot Robot
        { get => Elemento.Robot; }
        public QA_WebDominio(QA_WebElemento prmElemento)
        {
            Elemento = prmElemento;

            lista = new xLista();

        }
        public void Setup(string prmLista, string prmSintaxe)
        {

            lista.Importar(prmLista);

            filtro = prmSintaxe;

        }
        public bool SetAction(string prmValor)
        {

            if (GetElementos())
            {

                xLista fluxo = new xLista(prmValor, prmSeparador: "+");

                foreach (string item in fluxo)
                {
                    if (!SetFluxo(item))
                        Robot.Debug.Erro("Domínio não encontrado na lista ... " + item);
                }

            }
            else
                Robot.Debug.Erro("Busca de Domínios falhou ... " + GetXPath());

            return (false);
        }
        private bool SetFluxo(string prmFluxo)
        {

            int indice = lista.GetContain(prmFluxo.Trim());

            if (IsIndiceOk(indice))
            {
                GetElemento(indice).Click();

                return (true);
            }
            return (false);
        }
        private bool GetElementos()
        {

            Opcoes = Robot.GetElementsByXPath(GetXPath());

            if (!(Opcoes == null))
                return (Opcoes.Count > 0);

            return (false);
        }
        private IWebElement GetElemento(int prmIndice)
        {

            return (Opcoes[prmIndice - 1]);
        }
        private string GetXPath()
        {
            string raiz_elemento = "//*[@{0}='{1}']";

            if (filtro == null)
                return (string.Format(raiz_elemento, Chave.tag, Chave.valor));

            return (string.Format(raiz_elemento + filtro, Chave.tag, Chave.valor));

        }
        public bool IsIndiceOk(int prmIndice)
        {

            if (!(prmIndice == 0))
                return (Opcoes.Count >= prmIndice);
            return (false);
        }

        public bool IsON()
        { return (lista.IsOK); }
    }
    public class QA_WebTarget
    {
        private QA_WebElemento Elemento;

        public IWebElement control;

        public xTupla Chave
        { get => Elemento.Chave; }
        public QA_WebRobot Robot
        { get => Elemento.Robot; }

        public QA_WebTarget(QA_WebElemento prmElemento)
        {
            Elemento  = prmElemento;

            Setup(Chave.tag, Chave.valor);

        }
        private void Setup(string prmTag, string prmValor)
        {
            
            control = null;

            switch (prmTag.ToLower())
            {

                case "id":
                    control = Robot.GetElementBy(By.Id(prmValor));
                    break;

                case "name":
                    control = Robot.GetElementBy(By.Name(prmValor));
                    break;

                case "css":
                    control = Robot.GetElementBy(By.CssSelector(prmValor));
                    break;

                case "xpath":
                    control = Robot.GetElementBy(By.XPath(prmValor));
                    break;

                case "link":
                    control = Robot.GetElementBy(By.LinkText(prmValor));
                    break;

                default:
                    Elemento.Erro(prmErro: "TARGET não encontrado ... " + prmTag);
                    break;
            }
        }
    }
     public class QA_WebAtributos
    {

        private QA_WebTarget Target;
        public QA_WebAtributos(QA_WebTarget prmTarget)
        {
            Target = prmTarget;

        }
        private IWebElement control
        { get => Target.control; }
        public bool IsVisivel
        { get => control.Displayed; }
        public bool IsHabilitado
        { get => control.Enabled; }
        public Point area
        { get => control.Location; }
        public Size dimensao
        { get => control.Size; }
    }
    public class QA_WebMotor
    {

        public TestSuite Suite;

        private IWebDriver _driver;

        public QA_WebMotor(TestSuite prmSuite)
        { Suite = prmSuite; }

        private QA_WebRobot _robot;

        public QA_WebRobot Robot
        {
            get
            {
                if (_robot == null) { _robot = new QA_WebRobot(this); }
                return _robot;
            }
        }
        public IWebDriver driver
        {   
            get 
            {
                if (_driver == null)
                {
                    switch (Suite.tipoDriver)
                    {

                        case eTipoDriver.EdgeDriver:
                            _driver = new EdgeDriver();
                            break;

                        default:
                            _driver = new ChromeDriver();
                            break;

                    }

                }
                return _driver;
            }
        }

        public void Refresh()
        {

            Robot.Page.Refresh();
        }

        public IWebElement GetElementByXPath(string prmXPath)
        {
            try
            {
                return driver.FindElement(By.XPath(prmXPath));
            }
            catch (Exception e)
            {
                Robot.Debug.Erro(e);
            }
            return (null);
        }
        public ReadOnlyCollection<IWebElement> GetElementsByXPath(string prmXPath)
        {
            try
            {
                return driver.FindElements(By.XPath(prmXPath));
            }
            catch (Exception e)
            {
                Robot.Debug.Erro(e);
            }
            return (null);
        }
    public void Encerrar()
        { Robot.Quit(); }

    }
    public class QA_WebRobot
    {

        private QA_WebMotor Motor;

        public QA_WebPage Page;

        public QA_WebAction Action;

        public QA_WebLog Log;

        public QA_WebDebug Debug;

        public QA_DataPool Dados;

        public QA_WebRobot(QA_WebMotor prmMotor)
        {

            Motor = prmMotor;

            Dados = new QA_DataPool(this);

            Page = new QA_WebPage(this);

            Action = new QA_WebAction(this);

            Log = new QA_WebLog(this);

            Debug = new QA_WebDebug(this);

        }
        public IWebDriver driver
        {
            get { return Motor.driver; }
        }
        public QA_WebElemento Mapping(string prmKey, string prmTarget)
        {
            return (Page.AddItem(prmKey, prmTarget));
        }
        public void Input(string prmKey, string prmValor)
        {

            string valor = Dados.GetValor(prmKey, prmValor);

            Action.SetMap(prmKey, valor);
        }

        public bool GoURL(string prmUrl)
        {
            return (Action.GoURL(prmUrl));
        }

/*        public bool SetFocus(string prmTupla)
        {
            return (Action.SetFocus(prmTupla));
        }
        public void SetClick(string prmTarget)
        {
            Action.SetClick(prmTarget);
        }
        public void SetTexto(string prmTexto, string prmTarget)
        {
            Action.SetTexto(prmTexto, prmTarget);
        }*/
        public void Submit()
        { Page.Submit(); }

        public void Refresh()
        { driver.Navigate().Refresh(); }

        public void Quit()
        { driver.Quit(); }

        public void Pause(int prmSegundos)
        { Thread.Sleep(TimeSpan.FromSeconds(prmSegundos)); }
        public IWebElement GetElementBy(By prmTupla)
        {
            try
            {
                return driver.FindElement(prmTupla);
            }
            catch (Exception e)
            {
                Debug.Erro(e);
            }
            return (null);
        }
        public IWebElement GetElementByName(string prmName)
        {
            try
            {
                return driver.FindElement(By.Name(prmName));
            }
            catch (Exception e)
            {
                Debug.Erro(e);
            }
            return (null);
        }
        public IWebElement GetElementByXPath(string prmXPath)
        {
            try
            {
                return driver.FindElement(By.XPath(prmXPath));
            }
            catch (Exception e)
            {
                Debug.Erro(e);
            }
            return (null);
         }
        public ReadOnlyCollection<IWebElement> GetElementsByXPath(string prmXPath)
        {
            try
            {
                return driver.FindElements(By.XPath(prmXPath));
            }
            catch (Exception e)
            {
                Debug.Erro(e);
            }
            return (null);
        }

    }
    public class QA_WebDebug
    {
        private QA_WebRobot Robot;
        public QA_WebDebug(QA_WebRobot prmRobot)
        {

            Robot = prmRobot;

        }

        public void Erro(Exception prmErro)
        { Erro(prmErro.Message); }

        public void Erro(string prmErro)
        { Console(prmErro); }

        public void Stop()
        { Debug.Assert(false); }

        public void Stop(string prmTexto)
        { Console(prmTexto); Stop(); }

        public void Console(string prmTexto)
        { Debug.Print(prmTexto); }

    }
    public class QA_WebLog
    {
        private QA_WebRobot Robot;

        public QA_WebLog(QA_WebRobot prmRobot)
        {
            Robot = prmRobot;
        }
        public bool Aviso(string prmAviso)
        {
            Debug.Print(prmAviso);
            return false;
        }
        public bool Erro(string prmErro)
        {
            Debug.Print(prmErro);
            return false;
        }
    }
    public class QA_WebAction
    {
        private QA_WebRobot Robot;

        public QA_WebAction(QA_WebRobot prmRobot)
        {
            Robot = prmRobot;
        }
        private QA_WebPage Page
        {
            get { return Robot.Page; }
        }
        private QA_WebMapping Mapa
        {
            get { return Page.Mapa; }
        }
        private QA_WebElemento Elemento
        {
            get { return Page.Elemento; }
        }
        public bool GoURL(string prmUrl)
        {
            Robot.driver.Navigate().GoToUrl(prmUrl);
            return (true);
        }
        public bool SetFocus(string prmTarget)
        {
            return (Page.Mapa.SetTarget(prmTarget));
        }
        public bool SetMap(string prmKey, string prmValor)
        {
            if (Mapa.FindKey(prmKey))
            { return (Page.SetAction(prmValor)); }
            else
                return false;
        }
        public void SetClick(string prmTarget)
        {
            if (SetFocus(prmTarget))
            { Elemento.Click(); }
        }
        public void SetTexto(string prmTexto)
        {
            if (Mapa.TemElemento())
            { Elemento.SendKeys(prmTexto); }
        }
        public void SetTexto(string prmTexto, string prmTarget)
        {
            if (SetFocus(prmTarget))
            { Elemento.SendKeys(prmTexto); }
        }
    }
}






