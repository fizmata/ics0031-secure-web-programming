using System;
using System.Collections.Generic;
using System.Linq;


namespace ConsoleApp
{
    public static class PrimeCrypto
    {
	// Extended Euclidean algorithm
	private static int ExtendedEuclid(int a, int b)
	{
		var r = new []{a, b};
		while (r[1] != 0)
		{
			var q = r[0] / r[1];
			r = new []{r[1], r[0] - q * r[1]};
		}

		return r[0];
	}

	private static int InverseModulo(int a, int b)
	{
		var t = new int[]{0, 1};
		var r = new int[]{b, a};
		while (r[1] != 0)
		{
			var q = r[0] / r[1];
			t = new int[]{t[1], t[0] - q * t[1]};
			r = new int[]{r[1], r[0] - q * r[1]};
		}
		if (r[0] > 1) return 0;

		if (t[0] < 0) t[0] += b;

		return t[0];
	}

	private static int ModPow(int mod, int pow, long number)
	{
		long res = 1;
		number %= mod;
		while (pow > 0)
		{
			if (pow % 2 == 1)
			{
				res = (res * number) % mod;
			}

			pow >>= 1;
			number = (number * number) % mod;
		}

		return (int)res;
	}

	public static bool SingleTest(int n, int d, int r)
	{
		//pick a random integer a in the range [2, n − 2]
		// Just to make sure something weird doesn't happen
		var a = RandomNumberGenerator(0, n - 1);

		// x := (a ^ d)%n
		var x = ModPow(n, d, a);
		if (x == 1 || x == n - 1)
		{
			return true;
		}

		for (var j = 0; j < r - 1; j++)
		{
			//x = (x^2)%n
			x = (x * x) % n;
			if (x == n - 1)
			{
				return true;
			}
		}
		return false;
	}

	public static bool TestPrime(int n, int k)
	{
		// write n as (2^r)·d + 1 with d odd (by factoring out powers of 2 from n − 1)
		var (d, r) = FactorPossiblePrime(n);
		for (int i = 0; i < k; i++)
		{
			if (!SingleTest(n, d, r))
			{
				return false;
			}
		}

		return true;
	}

	public static bool TestPrimeOld(ulong possible)
	{
		if (possible % 2 == 0)
		{
			return false;
		}

		ulong sqrt = (ulong)(Math.Sqrt((double)possible));
		//test every odd number from 3 to sqrt the square root
		for (ulong i = 3; i <= sqrt; i += 2)
		{
			if (possible % i == 0)
			{
				return false;
			}
		}

		return true;
	}

	private static int RandomNumberGenerator(int min, int max)
	{
		var rand = new Random();
		return min < max ? rand.Next(min, max) : rand.Next(max, min);
	}

	public static int GeneratePrime()
	{
		var maxNum = (int)Math.Sqrt(int.MaxValue);
		var p = RandomNumberGenerator(0, maxNum);
		if (p % 2 == 0)
		{
			p--;
		}

		p %= maxNum;
		while (!TestPrime(p, 50))
		{
			p = RandomNumberGenerator(0, maxNum);
			if (p % 2 == 0)
			{
				p--;
			}

			p %= maxNum;
		}

		return p;
	}

	public static (int, int) FactorPossiblePrime(int n)
	{
		var r = 0;
		var d = n - 1;
		while (d % 2 != 1)
		{
			r++;
			d /= 2;
		}

		return (d, r);
	}

	public static List<int> FindPrimeFactors(int n)
	{
		var factors = new List<int>();
		if (n % 2 == 0)
		{
			factors.Add(2);
		}

		while (n % 2 == 0)
		{
			n /= 2;
		}

		for (var i = 3; i < (int)Math.Sqrt(n); i += 2)
		{
			if (n % i == 0)
			{
				factors.Add(i);
			}

			while (n % i == 0)
			{
				n /= i;
			}
		}

		if (n > 2)
		{
			factors.Add(n);
		}

		return factors;
	}

	public static int FindPrimitive(int n)
	{
		var phi = n - 1;
		var factors = FindPrimeFactors(phi);
		for (var r = 2; r <= phi; r++)
		{
			var isPrimitive = factors.All(factor => ModPow(n, phi / factor, r) != 1);
			if (isPrimitive) return r;
		}

		return 0;
	}

	// prime n primitive p
	public static bool CheckPrimitive(int n, int p)
	{
		if (p > n)
		{
			return false;
		}

		var phi = n - 1;
		var factors = FindPrimeFactors(phi);
		for (var r = 2; r <= phi; r++)
		{
			var isPrimitive = factors.All(factor => ModPow(n, phi / factor, r) != 1);
			if (isPrimitive && p == r) return true;
		}
		return false;
	}

	public static int GenerateCoPrime(int lambda)
	{
		var e = RandomNumberGenerator(0,lambda);
		while (ExtendedEuclid(e, lambda) != 1)
		{
			e = RandomNumberGenerator(0, lambda);
		}
		return e;
	}

	public static int GenerateLambda(int p, int q)
	{
		var lambda = 0;
		var lambdaBigEnough = false;
		while (!lambdaBigEnough)
		{
			// n := p * q
			var gcd = ExtendedEuclid(p - 1, q - 1);
			lambda = ((p - 1) * (q - 1)) / gcd;
			lambdaBigEnough = lambda > 2;
		}

		return lambda;
	}

	public static int[] GenerateRSA(int p = 0, int q = 0)
	{
		if (p == 0 || !TestPrime(p, 50))
		{
			p = GeneratePrime();
		}

		if (q == 0 || !TestPrime(q, 50))
		{
			q = GeneratePrime();
		}

		var lambda = GenerateLambda(p, q);
		var e = GenerateCoPrime(lambda);
		var d = InverseModulo(e,lambda);
		return new int[]{p * q, e, d};
	}

	public static (int, int, int, int, int, int) GenerateDH(int prime = 0, int primitive = 0, int userSecret = 0)
	{
		var maxNum = (int)Math.Sqrt(int.MaxValue);
		if (prime > maxNum)
		{
			throw new Exception("prime too big");
		}

		if (prime != 0 && !TestPrime(prime, 50))
		{
			throw new Exception("Provided number is not a prime");
		}

		if (prime == 0)
		{
			prime = GeneratePrime();
		}

		if (primitive == 0)
		{
			primitive = FindPrimitive(prime);
		}

		if (!CheckPrimitive(prime, primitive))
		{
			throw new Exception("Prime and Primitive do not correspond to each other");
		}

		// Generate our number that we'll (in theory) keep secret
		// Used for generating our part of the shared secret
		var serverSecret = RandomNumberGenerator(0, maxNum);
		//our part of the shared secret that will go through an insecure medium
		var ourPartial = ModPow(prime, serverSecret, primitive);
		// This will gather user's secret number and generate their part of the shared secret
		// as well as generating THE shared secret
		var theirPartial = ModPow(prime, userSecret, primitive);
		var sharedOur = ModPow(prime, serverSecret, theirPartial);
		var sharedTheir = ModPow(prime, userSecret, ourPartial);
		if (sharedOur != sharedTheir)
		{
			throw new Exception("Unable to match computed secrets with each other");
		}

		return (prime, primitive, serverSecret, ourPartial, theirPartial, sharedOur);
	}
	
	public static string RsaEnc(string plaintext, int e, int n)
	{
		var byteData = new byte[plaintext.Length * 4];
		var codePoints = MyExtensions.UnicodeCodePoints(plaintext);
		for (var i =0;i<codePoints.Count; i++)
		{
			var encLetter = ModPow(n, e, codePoints[i]);
			var byteArr = BitConverter.GetBytes(encLetter);
			for (var j = 0; j < 4; j++) byteData[i*4 + j] = byteArr[j];
		}
		var b64 = Convert.ToBase64String(byteData);
		return b64;
	}

	public static string RsaDec(string ciphertext, int d, int n)
	{
		var plain = "";
		var byteData = Convert.FromBase64String(ciphertext);
            
		for (var i = 0; i < byteData.Length; i += 4)
		{
			byte[] uintBytes = {byteData[i], byteData[i+1], byteData[i+2], byteData[i+3] };
			var encLetter = BitConverter.ToInt32(uintBytes, 0);
			var letter = ModPow(n, d, encLetter);
			plain += char.ConvertFromUtf32(letter);
		}

		return plain;
	}
    }
}