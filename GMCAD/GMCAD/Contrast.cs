
namespace GMvSAPR
{
    class Contrast
    {
        public static uint ImageContrast(uint point, int poz, int lenght)
        {
            int R;
            int G;
            int B;

            var N = (100 / lenght) * poz; //кол-во процентов

            if (N >= 0)
            {
                if (N == 100) N = 99;
                R = (int)((((point & 0x00FF0000) >> 16) * 100 - 128 * N) / (100 - N));
                G = (int)((((point & 0x0000FF00) >> 8) * 100 - 128 * N) / (100 - N));
                B = (int)(((point & 0x000000FF) * 100 - 128 * N) / (100 - N));
            }
            else
            {
                R = (int)((((point & 0x00FF0000) >> 16) * (100 - (-N)) + 128 * (-N)) / 100);
                G = (int)((((point & 0x0000FF00) >> 8) * (100 - (-N)) + 128 * (-N)) / 100);
                B = (int)(((point & 0x000000FF) * (100 - (-N)) + 128 * (-N)) / 100);
            }

            //контролируем переполнение переменных
            if (R < 0) R = 0;
            if (R > 255) R = 255;
            if (G < 0) G = 0;
            if (G > 255) G = 255;
            if (B < 0) B = 0;
            if (B > 255) B = 255;

            point = 0xFF000000 | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

            return point;
        }

    }
}