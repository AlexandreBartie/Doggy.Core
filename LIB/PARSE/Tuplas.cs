using Dooggy.Lib.Generic;
using Dooggy.Lib.Vars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Parse
{
    public class myTupla
    {

        private string delimitadorInicial = "[";
        private string delimitadorFinal = "]";

        private string separador = "=";

        private string _tag;
        private string _valor;

        private string _bruto;
        private string _descricao;

        public string tag => _tag;
        public string valor => _valor;

        public string bruto => _bruto;
        public string descricao => _descricao;

        public string tag_sql => string.Format("{0} as {1}", var_sql, tag);
        public string var_sql => String.Format("#({0})", tag);
        public string valor_sql { get { if (TemValor) return valor; return "''"; } }

        public string log => GetLog();

        public bool TemTag => myString.IsFull(tag);
        public bool TemValor => !IsNull;
        private bool TemDados => TemTag & TemValor;
        private bool TemDescricao => (descricao != "");

        public bool IsNull => myString.IsNull(valor);

        public bool IsMatch(string prmTag) => (TemTag && myString.IsEqual(tag, prmTag));

        public myTupla(string prmTexto)
        {
            Parse(prmTexto);
        }
        public myTupla(string prmTexto, string prmSeparador)
        {

            separador = prmSeparador;

            Parse(prmTexto);

        }

        public void Parse(string prmTexto)
        {

            //
            // Get descricao Tupla (estão entre delimitadores pré-definidos)
            //

            _descricao = Bloco.GetBloco(prmTexto, delimitadorInicial, delimitadorFinal).Trim();

            //
            // Remove descricao da Tupla (para permitir identificar "tag" e "valor")
            //

            _bruto = Bloco.GetBlocoRemove(prmTexto, delimitadorInicial, delimitadorFinal);

            //
            // Identifica "tag" e "valor"
            //

            xLista lista = new xLista(bruto, separador);

            _tag = lista.first;

            if (lista.IsUnico)
                SetValue(prmValor: null);
            else
                SetValue(prmValor: lista.last);

        }

        public bool SetValue(string prmValor) { _valor = prmValor; return true; }

        public bool SetValue(myTupla prmTupla)
        {
            if (prmTupla.IsMatch(tag))
                return SetValue(prmTupla.valor);

            return (false);
        }

        private string GetLog()
        {
            string log = "";

            if (TemTag)
                log += tag;

            if (TemDados)
                log += @": '" + valor + "'";

            if (TemDescricao)
                log += " <" + descricao + ">";

            return log.Trim();

        }

    }

    public class myTuplas : List<myTupla>
    {

        private string separador = ",";

        public string key;

        public string group;

        public bool IsFull => (Count > 0);

        public string txt { get => GetTXT(); }
        public string sql { get => GetSQL(); }
        public string log { get => GetLOG(); }
        public bool IsMatch(string prmKey) => (myString.IsEqual(key, prmKey));
        public bool IsGroup(string prmGroup) => (myString.IsEqual(group, prmGroup));

        public myTuplas()
        {
        }
        public myTuplas(string prmTuplas)
        {
            Parse(prmTuplas);
        }
        public myTuplas(string prmTuplas, string prmSeparador)
        {
            Parse(prmTuplas, prmSeparador);
        }
        public myTuplas SetKey(string prmKey, string prmGroup) { key = prmKey; group = prmGroup;  return this; }
        public myTuplas SetSeparador(string prmSeparador) { separador = prmSeparador; return this; }

        public myTuplas Parse(string prmLista, string prmSeparador) { separador = prmSeparador; return Parse(prmLista); }

        public myTuplas Parse(string prmLista)
        {
            if (myString.IsFull(prmLista))
            {
                foreach (string item in new xLista(prmLista, separador))
                    AddTupla(new myTupla(item));
            }
            return (this);
        }
        public myTuplas Parse(myTuplas prmTuplas)
        {
            foreach (myTupla tupla in prmTuplas)
                AddTupla(tupla);

            return (this);
        }
        public void AddTupla(myTupla prmTupla)
        {
            if (prmTupla.TemTag)

                if (!SetValue(prmTupla))
                    this.Add(prmTupla);
        }

        public void SetValues(string prmValores)
        {

            if (myString.IsFull(prmValores))
            {
                int cont = 0;
                foreach (string item in new xLista(prmValores, separador))
                {
                    if (this.Count == cont) break;

                    this[cont].SetValue(item);

                    cont++;
                }

            }

        }

        public bool SetValue(myTupla prmTupla)
        {
            if (prmTupla.TemTag)
            {

                foreach (myTupla Tupla in this)
                {
                    if (Tupla.SetValue(prmTupla))
                        return true;
                }

            }
            return (false);
        }
        public myTuplas GetValues()
        {
            myTuplas tuplas = new myTuplas();

            foreach (myTupla tupla in this)
            {
                if (tupla.TemValor)
                    tuplas.Add(tupla);
            }
            return tuplas;
        }
        public bool IsContem(string prmItem)
        {

            foreach (myTupla tupla in this)
            {
                if (prmItem.ToLower().Contains(tupla.bruto.ToLower()))
                    return true;
            }

            return (false);
        }
        private string GetTXT()
        {
            xLista text = new xLista();

            foreach (myTupla tupla in this)
            {
                text.Add(tupla.tag);
            }
            return (text.txt());
        }
        private string GetSQL()
        {
            xLista text = new xLista();

            foreach (myTupla tupla in this)
            {
                text.Add(tupla.tag_sql);
            }
            return (text.txt());
        }
        private string GetLOG()
        {
            xMemo text = new xMemo();

            foreach (myTupla tupla in this)
            {
                text.Add(tupla.log);
            }
            return (text.txt(", "));
        }
    }

    public class myTuplasBox : List<myTuplas>
    {

        public string name;

        public string header => name + "," + columns;
        public string columns => GetTXT();
        public string columns_sql => GetSQL();

        public myTuplas AddItem(string prmKey) => AddItem(prmKey, prmGroup: "main");
        public myTuplas AddItem(string prmKey, string prmGroup) => AddItem(new myTuplas().SetKey(prmKey, prmGroup));
        public myTuplas AddItem(myTuplas Tuplas)
        {
            Add(Tuplas); return Tuplas;
        }

        public myTuplasBox Main => Filter(prmGroup: "main");
        public myTuplasBox Filter(string prmGroup)
        {
            myTuplasBox Box = new myTuplasBox();

            foreach (myTuplas Tuplas in this)
            {
                if (Tuplas.IsGroup(prmGroup))
                    Box.Add(Tuplas);
            }
            return Box;
        }
        public void SetValues(string prmValues)
        {
            myTuplas Inputs = new myTuplas(prmValues);
            
            foreach (myTupla tupla in Inputs)
                SetValue(tupla);
        }
        private bool SetValue(myTupla prmTupla)
        {
            foreach (myTuplas Tuplas in this)
                if (Tuplas.SetValue(prmTupla)) return true;

            return false;
        }
        private string GetTXT()
        {
            xLista text = new xMemo();

            foreach (myTuplas Tuplas in this)
                if (Tuplas.IsFull)
                    text.Add(Tuplas.txt);

            return text.txt();
        }
        private string GetSQL()
        {
            xLista text = new xMemo();

            foreach (myTuplas Tuplas in this)
                text.Add(Tuplas.sql);

            return text.txt();
        }
    }

}
