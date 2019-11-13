object SpecialMath {
   def main(args: Array[String]) {
      println(SpecialMath(args(0).toInt));
   }

   def SpecialMath(n: Int): Long = {
       //println(n);
       var minus2 = 0;
       var minus1 = 0;
       var current = 1;
       var total : Long = 0;
       for (i <- n to 1 by -1) {
           //println(total + "+(" + i + "*"+ current + ")");
           total = total + (i * current);
           minus2 = minus1;
           minus1 = current;
           current = minus1 + minus2;
       }
       return total;
   }


   def SpecialMathRecursive(n: Int): Int = {
       if (n == 0){
           return 0;
       } else if (n == 1){
           return 1;
       }
       else{
           return n + SpecialMathRecursive(n-1) + SpecialMathRecursive(n-2);
       }
   }
}
