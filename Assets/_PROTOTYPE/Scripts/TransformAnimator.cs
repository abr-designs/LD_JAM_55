using System.Collections;
using UnityEngine;

public class TransformAnimator : MonoBehaviour
{

    [SerializeField, Header("Animation Time")]
    private Vector2 animationTimeRange;
    
    [SerializeField, Header("Scale Settings")]
    private AnimationCurve scaleCurve;
    
    [SerializeField, Min(0), Header("Rotation Settings")]
    private float rotationOffset;
    [SerializeField]
    private AnimationCurve rotationCurve;

    
    private bool _isPlaying;
    private bool _looping;
    private Vector3 _originalScale;
    private Quaternion _originalRotation;

    public void Play()
    {
        if (_isPlaying)
            Stop();

        var time = Random.Range(animationTimeRange.x, animationTimeRange.y);
        StartCoroutine(PlayAnimationCoroutine());

        return;
        //------------------------------------------------//
        IEnumerator PlayAnimationCoroutine()
        {
            _isPlaying = true;

            _originalScale = transform.localScale;
            _originalRotation = transform.rotation;

            var cor1 = StartCoroutine(ScaleAnimationCoroutine(time));
            var cor2 = StartCoroutine(RotationAnimationCoroutine(time));

            yield return new WaitForSeconds(time);
        
            StopCoroutine(cor1);
            StopCoroutine(cor2);

            transform.localScale = _originalScale;
            transform.rotation = _originalRotation;

            _isPlaying = false;
        }
    }

    public void Loop()
    {
        if (_isPlaying)
            Stop();

        _looping = true;
        StartCoroutine(PlayAnimationCoroutine());
        
        return;
        IEnumerator PlayAnimationCoroutine()
        {
            _isPlaying = true;

            _originalScale = transform.localScale;
            _originalRotation = transform.rotation;
            
            Coroutine cor1 = null;
            Coroutine cor2 = null;

            while (_looping)
            {
                var time = Random.Range(animationTimeRange.x, animationTimeRange.y);
                
                cor1 = StartCoroutine(ScaleAnimationCoroutine(time));
                cor2 = StartCoroutine(RotationAnimationCoroutine(time));

                yield return new WaitForSeconds(time);
            }
        
            StopCoroutine(cor1);
            StopCoroutine(cor2);

            transform.localScale = _originalScale;
            transform.rotation = _originalRotation;

            _isPlaying = false;
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        transform.localScale = _originalScale;
        transform.rotation = _originalRotation;
        _isPlaying = false;
        _looping = false;
    }

    private IEnumerator ScaleAnimationCoroutine(float time)
    {
        var originalScale = transform.localScale;
        
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            var dt = t / time;
            transform.localScale = originalScale * scaleCurve.Evaluate(dt);
            
            yield return null;
        }
    }
    
    private IEnumerator RotationAnimationCoroutine(float time)
    {
        var originalRotation = transform.localRotation;

        var CCW = originalRotation * Quaternion.Euler(0, 0, -rotationOffset);
        var CW = originalRotation * Quaternion.Euler(0, 0, rotationOffset);
        
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            var dt = t / time;
            transform.localRotation = Quaternion.Lerp(CCW, CW, scaleCurve.Evaluate(dt) - 0.5f);
            
            yield return null;
        }
    }
}
