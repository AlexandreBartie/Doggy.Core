using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using Dooggy.KERNEL;
using Dooggy.LIB.PARSE;
using Dooggy.LIB.FILES;

namespace Dooggy
{
    public enum eTipoElemento : int
    {
        Input = 0,
        Opcao = 10,
        Botao = 20
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

        public QA_WebElemento Elemento { get => Mapa.Elemento; }

        public TestTrace Trace { get => Robot.Trace; }

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
            { Trace.Log.Erro(e.Message); }

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

        public string key { get => _key; }

        public eTipoElemento tipo { get => _tipo; }

        public xTupla Chave { get => _chave; }

        public IWebElement control { get => Target.control; }


        public string filltro;

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

        public QA_WebRobot Robot { get => Page.Robot; }
        public TestTraceAction Trace { get => Robot.Trace.Action; }

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

            return Trace.Log.Erro(prmErro: "AÇÃO não encontrada" + tipo.ToString());

        }

        public void Refresh() => _target = null;

        public bool Clear()
        {
            try
            { control.Clear(); return (true); }

            catch (Exception e)
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
                    Trace.ActionElement("Click", key);

                return (true); 
            
            }

            catch (Exception e)
            { Trace.ActionFail("Click", e); }

            return (false);
        }
        public bool SendKeys(string prmTexto)
        {
            try
            { 
                control.SendKeys(prmTexto);

                Trace.ActionElement("Input", key, prmTexto); 
                
                return (true); 
            
            }

            catch (Exception e)
            { Trace.ActionFail("SendKeys", e); }

            return (false);
        }

    }
    public class QA_WebDominio
    {
        private QA_WebElemento Elemento;

        private xLista lista = new xLista();

        private string filtro;

        private ReadOnlyCollection<IWebElement> Opcoes;
        public xTupla Chave { get => Elemento.Chave; }

        public QA_WebRobot Robot { get => Elemento.Robot; }

        public TestKernel Kernel { get => Robot.Kernel; }

        public QA_WebDominio(QA_WebElemento prmElemento)
        {
            Elemento = prmElemento;

        }
        public void Setup(string prmLista, string prmSintaxe)
        {

            lista.Parse(prmLista, ";");

            filtro = prmSintaxe;

        }
        public bool SetAction(string prmValor)
        {

            if (GetElementos())
            {

                xLista fluxo = new xLista(Kernel.GetAdicaoElementos(), prmValor);

                foreach (string item in fluxo)
                {
                    if (!SetFluxo(item))
                        Robot.Trace.Log.Erro("Domínio não encontrado na lista ... " + item);
                }

            }
            else
                Robot.Trace.Log.Erro("Busca de Domínios falhou ... " + GetXPath());

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
            string raiz_elemento = Kernel.GetXPathBuscaRaizElementos();

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

        public xTupla Chave { get => Elemento.Chave; }
        public QA_WebRobot Robot { get => Elemento.Robot; }

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
                    Elemento.Trace.TargetNotFound(prmTag);
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
    public class QA_WebRobot
    {

        private TestMotor Motor;

        public QA_WebPage Page;

        public QA_WebAction Action;

        public QA_MassaDados Massa;

        public QA_WebRobot(TestMotor prmMotor)
        {

            Motor = prmMotor;

            Massa = new QA_MassaDados(this);

            Page = new QA_WebPage(this);

            Action = new QA_WebAction(this);

        }
        public TestProject Projeto { get => Motor.Suite.Projeto; }

        public TestProject Dado { get => Projeto; }

        public TestKernel Kernel { get => Projeto.Kernel; }
            
        public TestTrace Trace { get  => Kernel.Trace; }

        public IWebDriver driver  { get => Motor.driver; }

        public QA_WebElemento Mapping(string prmKey, string prmTarget) => (Page.AddItem(prmKey, prmTarget));

        public void Input(string prmKey, string prmValor)
        {

            string valor = Massa.GetValor(prmKey, prmValor);

            Action.SetMap(prmKey, valor);
        }
        public bool GoURL(string prmUrl)
        {
            return (Action.GoURL(prmUrl));
        }
        public void Submit()
        { Page.Submit(); }

        public bool Refresh()
        {  
            try
            { driver.Navigate().Refresh(); return (true);  }

            catch
            { Debug.Assert(false);  }

            return (false);

        }
        public void Quit()
        { driver.Quit(); }

        public void Pause(int prmSegundos) { Kernel.Pause(prmSegundos); }

        public IWebElement GetElementBy(By prmTupla)
        {
            try
            {
                return driver.FindElement(prmTupla);
            }
            catch (Exception e)
            {
                Trace.Log.Erro(e);
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
                Trace.Log.Erro(e);
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
                Trace.Log.Erro(e);
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
                Trace.Log.Erro(e);
            }
            return (null);
        }

    }
    public class QA_MassaDados
    {
        private QA_WebRobot Robot;

        public QA_FonteDados Fonte;

        private DataViewConnection DefaultView;

        public xJSON JSON = new xJSON();

        private bool IsON;

        public QA_MassaDados(QA_WebRobot prmRobot)
        {

            Robot = prmRobot;

            Fonte = new QA_FonteDados(this);

        }

        public TestProject Project { get => Robot.Projeto; }
        private TestTrace Trace { get => Robot.Trace; }
        private DataPoolConnection Pool { get => Project.Pool; }

        public bool IsONLINE { get => IsON; }
        private bool IsSTATIC => (DefaultView == null);
        public bool IsOK { get => JSON.IsOK; }
        public bool IsCurrent { get => JSON.IsCurrent; }
   
        public bool SetView(string prmTag)
        {

            DefaultView = null;

            if (Pool.SetView(prmTag))
                DefaultView = Pool.DataViewCorrente;

            return (IsSTATIC);

        }
        public void Add(string prmFluxo)
        {
            if (IsSTATIC)
               JSON.Add(prmFluxo);
            else
                AddCombine(prmFluxo, prmMestre:DefaultView.json());
        }
        public void Add(string prmFluxo, string prmView)
        {
            AddCombine(prmFluxo, prmMestre: Pool.json(prmView));
        }
        private void AddCombine(string prmFluxo, string prmMestre)
        {
            JSON.Add(prmFluxo, prmMestre);
        }
        public bool Save()
        {
            IsON = true;

            if (!JSON.Save())
                { Trace.Log.Erro("ERRO{JSON:Save} " + JSON.fluxo); }

            return (JSON.IsOK);

        }
        public bool Next()
        {
            return (JSON.Next());
        }

        public string GetValor(string prmKey, string prmPadrao)
        {
            return (JSON.GetValor(prmKey, prmPadrao));
        }

    }

    public class QA_FonteDados
    {

        private QA_MassaDados Massa;

        private xFileTXT _FileTXT;

        private xFileJUnit _FileJUnit;

        private TestConfig Config { get => Massa.Project.Config; }

        public xFileTXT FileTXT
        { 
            get
            {
                if (_FileTXT is null)
                    _FileTXT = new xFileTXT();
                return (_FileTXT);
            }
            set
            {
                _FileTXT = value;
            }
        }
        public xFileJUnit FileJUnit
        {
            get
            {
                if (_FileJUnit is null)
                {
                    _FileJUnit = new xFileJUnit();
                    _FileJUnit.SetEncoding(Config.EncodedDataJUNIT);
                }
                return (_FileJUnit);
            }
            set
            {
                _FileJUnit = value;
            }
        }
        public QA_FonteDados(QA_MassaDados prmMassa)
        {

            Massa = prmMassa;

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
            try
            { Robot.driver.Navigate().GoToUrl(prmUrl); return (true); }

            catch
            { }

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






