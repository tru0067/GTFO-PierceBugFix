﻿using Iced.Intel;
using UnityEngine.Assertions;

namespace PierceBugFix;

public class PierceBugDisassembler
{
    public static unsafe IntPtr FindInc(IntPtr methodPointer, int index, int totalExpected, int widthExpected)
    {
        IntPtr instructionIP = IntPtr.Zero;  // Return value, initialized to null.

        // Set up the decoder to go through the instructions.
        StreamCodeReader streamCodeReader = new((Stream)new UnmanagedMemoryStream((byte*)methodPointer, 65536L, 65536L, (FileAccess)1));
        Decoder decoder = Decoder.Create(sizeof(void*) * 8, streamCodeReader);
        decoder.IP = (ulong)(long)methodPointer;
        Instruction instruction = new();
        decoder.Decode(out instruction);

        // We are looking for the `index`th `Inc` instruction in this method.
        int incCount = 0;
        while (instruction.Mnemonic != Mnemonic.Int3)
        // `Int3` is an opcode that is sometimes used to halt execution for a debugger. We
        // can be reasonably sure that it will appear after our method and never be inside
        // it.
        {
            if (instruction.Mnemonic == Mnemonic.Inc)
            {
                ++incCount;
                if (incCount == index)
                {
                    instructionIP = (IntPtr)(long)instruction.IP;

                    // Error handling.
                    if ((instruction.NextIP - instruction.IP) != (ulong)widthExpected)
                    {
                        Logger.Error("PierceBugFix found an instruction with an unexpected width, this probably means the method has changed in some way and we should avoid changing it.");
                        Environment.FailFast("PierceBugFix found an instruction with an unexpected width, this probably means the method has changed in some way and we should avoid changing it.");
                    }
                }
            }
            decoder.Decode(out instruction);
        }
        streamCodeReader.Stream.Dispose();

        // Error handling.
        if (incCount != totalExpected)
        {
            Logger.Error("PierceBugFix didn't find the correct number of `Inc` instructions, this probably means the method has changed in some way and we should avoid changing it.");
            Environment.FailFast("PierceBugFix didn't find the correct number of `Inc` instructions, this probably means the method has changed in some way and we should avoid changing it.");
        }
        if (instructionIP == IntPtr.Zero)
        {
            Logger.Error("PierceBugFix found a zero instruction int pointer for our `Inc` instruction, something has gone terribly wrong.");
            Environment.FailFast("PierceBugFix found a zero instruction int pointer for our `Inc` instruction, something has gone terribly wrong.");
        }
        return instructionIP;
    }
}
