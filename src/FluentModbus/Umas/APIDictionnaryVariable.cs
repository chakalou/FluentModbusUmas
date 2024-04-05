using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentModbusUmas.Umas
{
    /// <summary>
    /// Description variable récupérée du dictionnaire de variable
    /// </summary>
    public class APIDictionnaryVariable
    {
        String _name;
        DictionnaryVariableClassType _variabletype;
        Int16 _blockMemory;
        byte _relativeOffset;
        Int16 _baseoffset;
        int _valeur;
        /// <summary>
        /// Constructeur de APIDictionnaryVariable
        /// </summary>
        /// <param name="name">Nom de la variable</param>
        /// <param name="baseoffset">base offset associé à la variable</param>
        /// <param name="blockMemory">Block memory associé à la variable</param>
        /// <param name="relativeOffset">Offset relatif à la variable</param>
        /// <param name="variabletype">Type de variable</param>
        public APIDictionnaryVariable(string name, Int16 baseoffset, byte relativeOffset, Int16 blockMemory, DictionnaryVariableClassType variabletype)
        {
            _name = name;
            _variabletype = variabletype;
            _blockMemory = blockMemory;
            _baseoffset = baseoffset;
            _relativeOffset = relativeOffset;
            _valeur = -1;
        }
        /// <summary>
        /// Nom de la variable
        /// </summary>
        public string Name { get => _name; }

        /// <summary>
        /// Offset relatif à la variable
        /// </summary>
        public byte RelativeOffset { get => _relativeOffset; }
        /// <summary>
        /// Base offset assocé mémoire de la variable
        /// </summary>
        public Int16 Baseoffset { get => _baseoffset; }
        /// <summary>
        /// Bloc mémoire où lire la variable
        /// </summary>
        public Int16 BlockMemory { get => _blockMemory; }
        /// <summary>
        /// Valeur de la variable API
        /// </summary>
        public int Valeur { get => _valeur; set => _valeur = value; }
        /// <summary>
        /// Type de variable API
        /// </summary>
        public DictionnaryVariableClassType Variabletype { get => _variabletype; }

        /// <summary>
        /// Fontion qui retourne la taille de la variable suivant son type
        /// </summary>
        /// <returns></returns>
        public int VariableLength
        {
            get
            {
                int ret = 0;
                switch (Variabletype)
                {
                    case DictionnaryVariableClassType.BOOL:
                    case DictionnaryVariableClassType.BYTE:
                    case DictionnaryVariableClassType.EBOOL:
                    case DictionnaryVariableClassType.CTU:
                        ret = 1;
                        break;
                    case DictionnaryVariableClassType.UINT:
                    case DictionnaryVariableClassType.INT:
                    case DictionnaryVariableClassType.WORD:
                        ret = 2;
                        break;
                    case DictionnaryVariableClassType.DINT:
                    case DictionnaryVariableClassType.UDINT:
                    case DictionnaryVariableClassType.REAL:
                    case DictionnaryVariableClassType.DATE:
                    case DictionnaryVariableClassType.TOD:
                    case DictionnaryVariableClassType.DT:
                    case DictionnaryVariableClassType.DWORD:
                        ret = 3;
                        break;
                    case DictionnaryVariableClassType.TIME:
                        ret = 4;
                        break;
                    case DictionnaryVariableClassType.STRING:
                        ret = 17;
                        break;
                    default:
                        ret = 0;
                        break;
                }

                return ret;


            }
        }
    }
}
