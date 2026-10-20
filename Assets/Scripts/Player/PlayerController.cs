using UnityEngine;
// Controlador modular para Unity 6.3. La física impide que Mathi atraviese el suelo.
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed=5.5f;
    [SerializeField] private bool isTopDown=false;
    private Rigidbody2D rb;
    private Animator animador;
    private Vector2 movimiento;
    private bool haciaDerecha=true;
    private void Awake()
    {
        rb=GetComponent<Rigidbody2D>();if(rb==null)rb=gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType=RigidbodyType2D.Dynamic;
        rb.gravityScale=isTopDown ? 0f : 3f;
        rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
        rb.interpolation=RigidbodyInterpolation2D.Interpolate;
        rb.constraints=RigidbodyConstraints2D.FreezeRotation;
        CapsuleCollider2D collider=GetComponent<CapsuleCollider2D>();
        if(collider==null)collider=gameObject.AddComponent<CapsuleCollider2D>();
        collider.isTrigger=false;collider.size=new Vector2(.57f,.89f);collider.offset=new Vector2(0,.45f);
        animador=GetComponent<Animator>();
    }
    private void Update()
    {
        bool bloqueado=MathiUI.Instancia!=null && MathiUI.Instancia.Bloqueado;
        float x=bloqueado ? 0f : Input.GetAxisRaw("Horizontal");
        float y=bloqueado ? 0f : (isTopDown ? Input.GetAxisRaw("Vertical") : 0f);
        movimiento=new Vector2(x,y).normalized;
        if(x>0 && !haciaDerecha) Voltear();else if(x<0 && haciaDerecha)Voltear();
        if(animador!=null)animador.SetFloat("Speed",movimiento.sqrMagnitude);
    }
    private void FixedUpdate()
    {
        if(rb!=null) rb.linearVelocity=isTopDown ? movimiento*moveSpeed :
            new Vector2(movimiento.x*moveSpeed,rb.linearVelocity.y);
    }
    private void Voltear(){haciaDerecha=!haciaDerecha;var s=transform.localScale;s.x*=-1;transform.localScale=s;}
}
