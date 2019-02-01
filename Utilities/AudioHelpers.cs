using System.Linq;

namespace AudioMarcoPolo.Utilities
{
    public static class AudioHelpers
    {
        public static int PPCM(int[] numbers)
        {
            if (numbers.Length == 0) return 0;
            if (numbers.Length < 2) return numbers[0];
            return PPCM(numbers[0], PPCM(numbers.Skip(1).ToArray()));
        }
        public static int PPCM(int a, int b)
        {
            if (b == 0) return a;
            if (a == 0) return b;
            return (a * b) / PGCD(a, b);
        }

        public static int PGCD(int[] numbers)
        {
            if (numbers.Length == 0) return 0;
            if (numbers.Length < 2) return numbers[0];
            return PGCD(numbers[0], PGCD(numbers.Skip(1).ToArray()));
        }
        public static int PGCD(int a, int b)
        {
            var firstNumber = a;
            var secondNumber = b;

            var gcd = 0;
            if (firstNumber > secondNumber)
            {
                var aa = secondNumber;
                secondNumber = firstNumber;
                firstNumber = aa;
            }

            while (secondNumber != 0)
            {
                gcd = firstNumber % secondNumber;
                firstNumber = secondNumber;
                if (gcd == 0)
                {
                    gcd = secondNumber;
                    secondNumber = 0;  // cleanly exit the loop
                    //break;  // I do not like doing this.
                }
                else
                {
                    secondNumber = gcd; // do this only if we need to loop again
                }
            }
            return gcd;
        }
    }
}
