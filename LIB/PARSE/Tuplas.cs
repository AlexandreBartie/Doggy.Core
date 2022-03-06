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

        private string delimitadorDestaque = ":";

        private string separador = "=";

        private string _name;
        private string _value;

        private string _tag;
        private string _format;

        private string _bruto;

        public string name => _name;
        public string tag => _tag;

        private string format => _format;
        public string value => _value;


        public string bruto => _bruto;

        public string name_sql => GetSQL();
        public string var_sql => String.Format("#({0})", name);
        public string value_sql => GetValue();
        public string mask => GetMask();
        public string log => GetLog();

        public bool TemKey => myString.IsFull(name);
        public bool TemValue => !IsNull;
        public bool TemVariavel => TemValue || TemTag;
        private bool TemDados => TemKey & TemValue;
        
        private bool TemTag => (tag != "");
        private bool TemFormat => (format != "");
        public bool IsNull => myString.IsNull(value);

        public bool IsMatch(string prmName) => (TemKey && myString.IsEqual(name, prmName));

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

            string destaque;

            //
            // Get destaque Tupla (estão entre delimitadores pré-definidos)
            //

            destaque = Bloco.GetBloco(prmTexto, delimitadorInicial, delimitadorFinal).Trim();

            _tag = myString.GetFirst(destaque, prmDelimitador: delimitadorDestaque);

            _format = myString.GetLast(destaque, prmDelimitador: delimitadorDestaque);

            //
            // Remove descricao da Tupla (para permitir identificar "tag" e "valor")
            //

            _bruto = Bloco.GetBlocoRemove(prmTexto, delimitadorInicial, delimitadorFinal);

            //
            // Identifica "tag" e "valor"
            //

            xLista lista = new xLista(bruto, separador);

            _name = lista.first;

            if (lista.IsUnico)
                SetValue(prmValue: null);
            else
                SetValue(prmValue: lista.last);

        }

        public bool SetValue(string prmValue) { _value = prmValue; return true; }

        public bool SetValue(myTupla prmTupla)
        {
            if (prmTupla.IsMatch(name))
                return SetValue(prmTupla.value);

            return (false);
        }

        private string GetLog()
        {
            string log = "";

            if (TemKey)
                log += name;

            if (TemDados)
                log += @": '" + value+ "'";

            if (TemTag)
                log += " <" + tag + ">";

            if (TemFormat)
                log += " <" + format + ">";

            return log.Trim();
        }

        private string GetSQL()
        {
            if (TemVariavel)
                return string.Format("{0} as {1}", var_sql, name);

            return name;
        }

        private string GetValue()
        {
            if (TemValue) return value;

            if (TemTag) return tag;

            return "''";
        }

        private string GetMask()
        {
            if (TemFormat) return string.Format("{0} = {1}", name, format);

            return "";
        }
    }

    public class myTuplas : List<myTupla>
    {

        private string separador = ",";

        public string key;

        public string group;

        public bool IsFull => !IsEmpty;
        public bool IsEmpty => (Count == 0);

        public string txt { get => GetTXT(); }
        public string sql { get => GetSQL(); }
        public string log { get => GetLOG(); }
        public string mask { get => GetMask(); }

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
            if (prmTupla.TemKey)

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
            if (prmTupla.TemKey)
            {

                foreach (myTupla Tupla in this)
                {
                    if (Tupla.SetValue(prmTupla))
                        return true;
                }

            }
            return (false);
        }
        public string GetValue(string prmName) => GetValue(prmName, prmPadrao: "");
        public string GetValue(string prmName, string prmPadrao)
        {
            foreach (myTupla Tupla in this)
            {
                if (Tupla.IsMatch(prmName))
                    return Tupla.value;
            }
            return (prmPadrao);
        }
        
        public myTuplas GetVariavel()
        {
            myTuplas tuplas = new myTuplas();

            foreach (myTupla tupla in this)
            {
                if (tupla.TemVariavel)
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
                text.Add(tupla.name);
            }
            return (text.txt);
        }
        private string GetSQL()
        {
            xLista text = new xLista();

            foreach (myTupla tupla in this)
            {
                text.Add(tupla.name_sql);
            }
            return (text.csv);
        }
        private string GetLOG()
        {
            xMemo text = new xMemo();

            foreach (myTupla tupla in this)
            {
                text.Add(tupla.log);
            }
            return (text.Export(", "));
        }
        private string GetMask()
        {
            xMemo text = new xMemo();

            foreach (myTupla tupla in this)
            {
                text.Add(tupla.mask);
            }
            return (text.csv);
        }

    }

    public class myTuplasBox : List<myTuplas>
    {

        public string name;

        public string header => name + "," + columns;
        public string columns => GetTXT();
        public string columns_sql => GetSQL();
        public string mask => GetMask();

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

            return text.txt;
        }
        private string GetSQL()
        {
            xLista text = new xMemo();

            foreach (myTuplas Tuplas in this)
                if (Tuplas.IsFull)
                    text.Add(Tuplas.sql);

            return text.csv;
        }
        private string GetMask()
        {
            xLista text = new xMemo();

            foreach (myTuplas Tuplas in this)
                if (Tuplas.IsFull)
                    text.Add(Tuplas.mask);

            return text.csv;
        }
    }

}
