using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Parser
{
    public enum ParseType
    {
        VAR,
        A_OP,
        INC,
        DEC,
        OP,
        L_OP,
        C_OP,
        DIGIT,
        BOOLEAN,
        LB,
        LB_S,
        PRINT,
        COND_TRAN,
        UNCOND_TRAN,
        LABEL,
        UNCOND_TRAN_L,
        LIST,
        CREATE_LIST,
        HASH_SET,
        CREATE_HASH_SET,
        ADD,
        DELETE,
        CONTAINS,
        INDEX,
        TRIAD_LINK,
        NULL,
        FUNC_NAME,
        FUNC_ARG_BEGIN,
        FUNC_ARG_CLOSE,
        FUNC_ARG_DELIMITER,
        ASYNC
    }
}
