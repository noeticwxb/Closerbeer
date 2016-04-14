using UnityEngine;
using System.Collections;

[AddComponentMenu("AnimatedTexture/AnimatedTextureUV")]
public class AnimatedTextureUV : MonoBehaviour 
{
	//vars for the whole sheet
	public int m_ColCount =  4;
	public int m_RowCount =  4;
	 
	//vars for animation
	public int m_StartRowIndex  =  0; //Zero Indexed
	public int m_StartColIndex = 0; //Zero Indexed
	public int m_TotalCells = 4;
	public int m_fps     = 10;
	//Maybe this should be a private var
	private Vector2 m_Offset;
	
	private Renderer _myRenderer;
	
	//
	void Start()
	{
		_myRenderer	=	renderer;
	}
	
	//Update
	void Update() 
	{ 
		SetSpriteAnimation(m_ColCount, m_RowCount, m_StartRowIndex, m_StartColIndex, m_TotalCells, m_fps);  
	}
	 
	//SetSpriteAnimation
	void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps )
	{	 
		if( null == _myRenderer || null == _myRenderer.material )
			return;
		
	    // Calculate index
	    int index  = (int)(Time.time * fps);
	    // Repeat when exhausting all cells
	    index = index % totalCells;
	 
	    // Size of every cell
	    float sizeX = 1.0f / colCount;
	    float sizeY = 1.0f / rowCount;
	    Vector2 size =  new Vector2(sizeX,sizeY);
	 
	    // split into horizontal and vertical index
	    int uIndex = index % colCount;
	    int vIndex = index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex+colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY);
	 
	    _myRenderer.material.SetTextureOffset ("_MainTex", offset);
	    _myRenderer.material.SetTextureScale  ("_MainTex", size);
	}
}
