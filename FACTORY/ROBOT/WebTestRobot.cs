using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using Katty;

namespace Dooggy
{
    public enum eTipoElemento : int
    {
        Input = 0,
        Opcao = 10,
        Botao = 20
    }
    public class WebMotor
    {

        public RobotSuite Suite;

        public WebRobot Robot;

        public IWebDriver WebDriver;
        public bool IsWorking => (WebDriver != null);

        private TestTrace Trace => Suite.Trace;

        public WebMotor(RobotSuite prmSuite)
        { 
            Suite = prmSuite;

            Robot = new WebRobot(this);

            WebDriver = GettWebDriver(prmPathDriver: Robot.Projeto.pathWebDriver);
        }

        public void Refresh() => Robot.Page.Refresh();
        public void Encerrar() => Robot.Quit();

        private IWebDriver GettWebDriver(string prmPathDriver)
        {
            try
            {
                switch (Suite.Projeto.tipoDriver)
                {
                    case eTipoDriver.EdgeDriver:
                        //_driver = new EdgeDriver();
                        break;

                    default:
                        return new ChromeDriver(prmPathDriver);
                }
            }
            catch (Exception e)
            { Trace.Erro.msgErro(string.Format("Driver não localizado ... -path: {0} -error: {1}", prmPathDriver, e.Message)); }

            return null;
        }

    }
    public class WebRobot
    {

        private WebMotor Motor;

        public RobotData Massa;

        public WebPage Page;

        public WebAction Action;

        public IWebDriver WebDriver => Motor.WebDriver;

        public RobotProject Projeto => Motor.Suite.Projeto;
        public TestTrace Trace => Projeto.Trace;

        public WebElemento Mapping(string prmKey, string prmTarget) => (Page.AddItem(prmKey, prmTarget));

        public WebRobot(WebMotor prmMotor)
        {

            Motor = prmMotor;

            Massa = new RobotData(this);

            Page = new WebPage(this);

            Action = new WebAction(this);

        }

        public void Input(string prmKey, string prmValor)
        {
            string valor = Massa.GetValor(prmKey, prmValor);

            Action.SetMap(prmKey, valor);
        }

        public void Click(string prmKey)
        {
            Action.SetClick(prmKey);
        }

        public bool GoURL(string prmUrl) => Action.GoURL(prmUrl);

        public void Submit() => Page.Submit();

        public bool Refresh()
        {
            try
            { WebDriver.Navigate().Refresh(); return (true); }
            
            catch (Exception e)
            { Trace.Erro.msgErro(e.Message); }

            return (false);

        }
        public void Quit() => WebDriver.Quit();

        public void Pause(int prmSegundos) => Projeto.Pause(prmSegundos); 

        public IWebElement GetElementBy(By prmTupla)
        {
            try
            { return WebDriver.FindElement(prmTupla); }
            
            catch (Exception e)
            { Trace.Erro.msgErro(e); }
            
            return (null);
        }
        public IWebElement GetElementByName(string prmName)
        {
            try
            { return WebDriver.FindElement(By.Name(prmName)); }

            catch (Exception e)
            { Trace.Erro.msgErro(e); }

            return (null);
        }
        public IWebElement GetElementByXPath(string prmXPath)
        {
            try
            { return WebDriver.FindElement(By.XPath(prmXPath)); }
            
            catch (Exception e)
            { Trace.Erro.msgErro(e); }

            return (null);

        }
        public ReadOnlyCollection<IWebElement> GetElementsByXPath(string prmXPath)
        {
            try
            { return WebDriver.FindElements(By.XPath(prmXPath)); }
            
            catch (Exception e)
            { Trace.Erro.msgErro(e); }

            return (null);

        }

    }
    public class WebPage
    {

        public WebRobot Robot;

        public WebMapping Mapa;

        private IWebElement LastControl;
        private bool TemLastControl => !(LastControl == null);

        public WebPage(WebRobot prmRobot)
        {
            Robot = prmRobot;

            Mapa = new WebMapping(this);
        }

        public WebElemento Elemento { get => Mapa.Elemento; }

        public TestTrace Trace { get => Robot.Trace; }

        public WebElemento AddItem(string prmKey, string prmTarget)
        {
            return (Mapa.AddKey(prmKey, prmTarget));
        }
        public bool SetAction(string prmValor)
        {

            if (Elemento.SetAction(prmValor))
                { LastControl = Elemento.control; return (true); }

            return (false);
        }

        public bool Submit()
        {
            try
            {
                if (TemLastControl)
                { LastControl.Submit(); return (true); }
            }

            catch (Exception e)
            { Trace.LogRobot.msgErro(e.Message); }

            return (false);
        }
        public void Refresh() => Mapa.Refresh();

    }
    public class WebMapping
    {
        
        private WebPage Page;

        public List<WebElemento> Elementos;

        public WebMapping(WebPage prmPage)
        {

            Page = prmPage;

            Elementos = new List<WebElemento> { };

        }

        private WebElemento _elemento;

        public WebElemento Elemento
        { get => _elemento; }

        public WebElemento AddKey(string prmKey, string prmTarget)
        {

            WebElemento elemento = new WebElemento(Page, prmKey, prmTarget);

            Elementos.Add(elemento);

            return (elemento);
        }
        public bool FindKey(string prmKey)
        {

            _elemento = null;

            foreach (WebElemento item in Elementos)
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

            myTupla Target = new myTupla(prmTarget);

            return Elemento.SetAction(Target.value);
        }
        public void Refresh()
        {

            _elemento = null;

            foreach (WebElemento item in Elementos)
                item.Refresh();
        }

        public bool TemElemento()
        { return !(_elemento == null); }
    }
    public class WebElemento
    {

        public WebPage Page;

        private string _key;

        private eTipoElemento _tipo = eTipoElemento.Input;

        private WebDominio Dominio;

        private WebTarget _target;

        public myTupla chave;

        public string key => _key;

        public eTipoElemento tipo => _tipo;

        public IWebElement control => Target.control;

        public bool TemControl => (control != null);

        public string filltro;

        public WebElemento(WebPage prmPage, string prmKey, string prmTarget)
        {
            Page = prmPage;

            _key = prmKey.ToLower();
            _tipo = eTipoElemento.Input;

            chave = new myTupla(prmTarget);

            Dominio = new WebDominio(this);

        }

        public WebTarget Target
        { get
            {
                if (_target == null)
                { _target = new WebTarget(this); }
                return _target;
            }
        }

        public WebRobot Robot { get => Page.Robot; }
        public RobotProject Projeto { get => Robot.Projeto; }
        public TestTrace Trace { get => Robot.Trace; }

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

            if (Dominio.IsFull())
            {
                return (Dominio.SetAction(key, prmValor));
            }
            else
            {
                switch (tipo)
                {

                    case eTipoElemento.Input:
                        {
                            if (Click(prmFake: true))
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

            Trace.Erro.msgErro(prmTexto: "AÇÃO não encontrada" + tipo.ToString());

            return (false);

        }

        public void Refresh() => _target = null;

        public bool Clear()
        {
            try
            { control.Clear(); return (true); }

            catch //(Exception e)
            { };

            return (false);
        }

        public bool Click() => Click(prmFake: false);

        public bool Click(bool prmFake)
        {
            try
            { 
                control.Click();

                if (!prmFake )
                    Trace.LogRobot.ActionElement("Click", key);

                return (true);           
            }

            catch (Exception e)
            { Trace.LogRobot.ActionFail("Click", e); }

            return (false);
        }
        public bool SendKeys(string prmTexto)
        {
            try
            { 
                control.SendKeys(prmTexto);

                Trace.LogRobot.ActionElement("Input", key, prmTexto); 
                
                return (true); 
            
            }

            catch (Exception e)
            { Trace.LogRobot.ActionFail("SendKeys", e); }

            return (false);
        }

    }
    public class WebDominio
    {
        private WebElemento Elemento;

        public myTupla chave { get => Elemento.chave; }
        public WebRobot Robot { get => Elemento.Robot; }
        public RobotProject Projeto { get => Robot.Projeto; }
        public TestTrace Trace { get => Robot.Trace; }

        private xLista lista = new xLista();

        private string filtro;

        private ReadOnlyCollection<IWebElement> Opcoes;

        public WebDominio(WebElemento prmElemento)
        {
            Elemento = prmElemento;

        }
        public void Setup(string prmLista, string prmSintaxe)
        {

            lista.Parse(prmLista, "+");// ");");

            filtro = prmSintaxe;

        }
        public bool SetAction(string prmKey, string prmValor)
        {

            if (GetElementos())
            {

                xLista Flow = new xLista(prmValor, prmSeparador: Projeto.Parameters.GetAdicaoElementos());

                foreach (string item in Flow)
                {
                    if (SetFlow(item))
                        Trace.LogRobot.ActionElement("Select", prmKey, item);
                    else
                        Trace.Erro.msgErro("Domínio não encontrado na lista ... " + item);
                }

            }
            else
                Trace.Erro.msgErro("Busca de Domínios falhou ... " + GetXPath());

            return (false);
        }
        private bool SetFlow(string prmFlow)
        {

            int indice = lista.GetContem(prmFlow.Trim());

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
            string raiz_elemento = Projeto.Parameters.GetXPathBuscaRaizElementos();

            if (filtro == null)
                return (string.Format(raiz_elemento, chave.name, chave.value));

            return (string.Format(raiz_elemento + filtro, chave.name, chave.value));

        }
        public bool IsIndiceOk(int prmIndice)
        {

            if (!(prmIndice == 0))
                return (Opcoes.Count >= prmIndice);
            return (false);
        }

        public bool IsFull()
        { return (lista.IsFull); }
    }
    public class WebTarget
    {
        private WebElemento Elemento;

        public IWebElement control;

        public myTupla chave { get => Elemento.chave; }
        public WebRobot Robot { get => Elemento.Robot; }

        public WebTarget(WebElemento prmElemento)
        {
            Elemento  = prmElemento;

            Setup(chave.name, chave.value);

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
                    Elemento.Trace.LogRobot.TargetNotFound(prmTag);
                    break;
            }
        }
    }
     public class WebAtributos
    {

        private WebTarget Target;

        private IWebElement control => Target.control;
        public Point area => control.Location;
        public Size dimensao => control.Size;
        public bool IsVisivel => control.Displayed;
        public bool IsHabilitado => control.Enabled;

        public WebAtributos(WebTarget prmTarget)
        {
            Target = prmTarget;

        }

    }
    public class WebAction
    {
        private WebRobot Robot;

        private TestTrace Trace => Robot.Trace;

        public WebAction(WebRobot prmRobot)
        {
            Robot = prmRobot;
        }
        private WebPage Page
        {
            get { return Robot.Page; }
        }
        private WebMapping Mapa
        {
            get { return Page.Mapa; }
        }
        private WebElemento Elemento
        {
            get { return Page.Elemento; }
        }
        public bool GoURL(string prmUrl)
        {
            try
            { Robot.WebDriver.Navigate().GoToUrl(prmUrl); return (true); }

            catch (Exception e)
            { Trace.Erro.msgErro(e.Message); }

            return (false);
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






