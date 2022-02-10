using Dooggy;
using Dooggy.Lib.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.Lib.Parse
{
    public class xParseCSV : xMemo
    {

        public string delimitador = "\"";

        public xParseCSV()
        { }
        public xParseCSV(string prmTexto)
        { new xParseCSV(prmTexto, separador); }

        public xParseCSV(string prmTexto, string prmSeparador)
        { separador = prmSeparador; Parse(prmTexto); }

        public override void Parse(string prmLista) => Parse(prmLista, separador, delimitador);
        public override void Parse(string prmLista, string prmSeparador) => Parse(prmLista, prmSeparador, delimitador);

        public void Parse(string prmLista, string prmSeparador, string prmDelimitador)
        {

            separador = prmSeparador;
            delimitador = prmDelimitador;

            if (prmLista.Trim() != "")
            {

                string texto;
                string lista = "";
                string resto = prmLista;

                bool IsDelimitedStartON = false;
                bool IsDelimitedEndON = false;

                foreach (string item in prmLista.Split(separador))
                {

                    bool IsCycleBegin = false;
                    bool IsCycleEnding = false;

                    bool IsDetectStart = false;
                    bool IsDetectEnd = false;

                    texto = item.Trim();

                    resto = GetSubstring(resto, prmIndice: item.Length + 1);


                    // Delimitador ISOLADO, tratamento especial
                    if (texto == delimitador)

                        // Delimitador START ON
                        if (IsDelimitedStartON)

                            // sinalizar fechar o ciclo
                            IsCycleEnding = true;

                        // Delimitador START OFF
                        else
                            // sinalizar iniciar o ciclo
                            IsCycleBegin = true;

                    else
                    {

                        // Detectar Delimitadores INICIAL e FINAL 
                        IsDetectStart = texto.StartsWith(delimitador);
                        IsDetectEnd = texto.EndsWith(delimitador);

                        // Ciclo: EM ABERTO
                        if ((IsDelimitedStartON))

                            // Delimitador FINAL detectado
                            if (IsDetectEnd)

                                //  sinalizar FECHAR CICLO
                                IsCycleEnding = true;

                            // Delimitador FINAL não detectado
                            else

                                //  manter espacos, pois o CICLO está em aberto
                                texto = item;

                        // Ciclo: NÃO ABERTO
                        else

                            // Delimitador INICIAL localizado: 
                            if ((IsDetectStart & !IsDetectEnd))

                            //sinalizar INICIAR CICLO (apenas se localizador delimitador FINAL no futuro)
                            IsCycleBegin = ExisteDelimitadorEND(resto);

                    }

                    // Ciclo recém INICIADO: retirar espacos brancos iniciais 
                    if (IsCycleBegin)
                    { texto = item.TrimStart(); IsDelimitedStartON = true; }

                    // Ciclo recém FECHADO: : retirar espacos brancos finais 
                    if (IsCycleEnding)
                    { texto = item.TrimEnd(); IsDelimitedEndON = true; }

                    // Delimitadores não ENCONTRADOS
                    if (!(IsDelimitedStartON | IsDelimitedEndON))
                        this.Add(texto);
                    else
                    {
                        // Delimitador INICIAL JÁ encontrado
                        if (IsDelimitedStartON)
                        {
                            if (lista == "")
                                lista += texto;
                            else
                                lista += separador + texto;
                        }

                        // Delimitador FINAL JÁ encontrado
                        if (IsDelimitedEndON)
                        { this.Add(lista); lista = ""; IsDelimitedStartON = false; IsDelimitedEndON = false; }

                    }

                }

                //if (lista != "")
                //    this.Add(lista);

            }

        }

        private string GetSubstring(string prmTexto, int prmIndice)
        {

            if (prmIndice <= prmTexto.Length)
                return prmTexto.Substring(prmIndice);

            return "";
        }

        private bool ExisteDelimitadorEND(string prmTexto)
        {
            string texto = prmTexto.Replace(" ", "");
            string alvo = delimitador + separador;

            if (texto.EndsWith(delimitador))
            { return (true); }


            return (texto.IndexOf(alvo) != -1);
        }


    }

}
