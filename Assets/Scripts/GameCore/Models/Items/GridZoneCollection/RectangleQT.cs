public struct RectangleQT
{
    //Координаты
    public int X;
    public int Y;

    //Размеры
    public int Width;
    public int Height;
    
    public RectangleQT(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    //границы области
    public int Left { get { return X; } }
    public int Right { get { return X + Width - 1; } }
    public int Top { get { return Y + Height - 1; } }
    public int Bottom { get { return Y; } }

    //проверка области на 0 размер
    public bool IsEmpty { get { return (Width == 0 || Height == 0); } }        

    public bool Contains(int x, int y)
    {
        return (x >= Left && x <= Right && y >= Bottom && y <= Top);
    }

    public bool IntersectsWith(RectangleQT rect)
    {
        //TODO: пересечение 2х прямоугольников (пока не актуально)
        return false;
    }

    public bool Contains(RectangleQT rect)
    {
        //TODO:область содержит в себе полностью другую область (пока не актуально)
        return false;
    }
}