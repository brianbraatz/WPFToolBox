var ceID;
var txtCommenterName, txtCommenterEmail, txtCommenterUrl, txtCommentBody, chkRemember, btnCommentAdd, commentContainer, commentInfo;

function initComments()
{
    txtCommenterName = document.getElementById('txtName');
    txtCommenterEmail = document.getElementById('txtEmail');
    txtCommenterUrl = document.getElementById('txtUrl');
    txtCommentBody = document.getElementById('txtBody');
    chkRemember = document.getElementById('chkRemember');
    btnCommentAdd = document.getElementById('btnAdd');
    commentContainer = document.getElementById('commentListContainer');
    commentInfo = document.getElementById('commentInfo');	   
}

function addComment()
{
    if(addCommentIsValid())
    {
        commentInfo.innerHTML = 'Adding comment...';
	    JundaWeb.Services.Comments.AddComment(ceID, txtCommenterName.value, txtCommenterEmail.value, txtCommenterUrl.value, escapeToBr(txtCommentBody.value), chkRemember.checked, addCommentSuccess, onTimeout, onRaiseError);
    }
    else
    {
        commentInfo.innerHTML = g_errors;
    }
}
function addCommentSuccess(res)
{
    if(res != "-1")
    {
        txtCommentBody.value = '';
        getComments(ceID);
    }
    else
    {
		commentInfo.innerHTML = '<span class="error">There was an error adding your comment</span>';
    }
}
function addCommentIsValid()
{
    g_errorCount = 0;
    g_errors = '<ul>'; 
    
    if(txtCommenterName.value == '')
    {
        g_errors += '<li>Name is required.</li>';
        g_errorCount++;
    }
    
    if(txtCommenterEmail.value == '')
    {
        g_errors += '<li>Email is required.</li>';
        g_errorCount++;
    }    
    if(txtCommentBody.value == '')
    {
        g_errors += '<li>Comment is required.</li>';
        g_errorCount++;
    }
    else if(txtCommentBody.value.length > 1500)
    {
        g_errors += '<li>Comment is too long.  1500 characters is the limit and you have ' + txtCommenterBody.value.length + '</li>';
        g_errorCount++;
    }
    
    if(g_errorCount > 0)
    {
        if(g_errorCount > 1)
            g_errors = '<span class="error">' + g_errorCount + ' errors found.' + g_errors + '</ul></span>';
        else
            g_errors = '<span class="error">' + g_errorCount + ' error found.' + g_errors + '</ul></span>';
        return false;
    }
    else
        return true;
}

function getComments()
{
    commentInfo.innerHTML = 'Refreshing comments...';
    JundaWeb.Services.Comments.GetCommentsHTML(ceID, getCommentsSuccess, onTimeout, onRaiseError);	
}
function getCommentsSuccess(res)
{
    if(res != "-1")
    {  
	    commentContainer.innerHTML = res;
	    commentInfo.innerHTML = 'The comment has been added, thanks for your input.';
	}
}
function deleteComment(commentID)
{
	JundaWeb.Services.Comments.DeleteComment(commentID, deleteCommentSuccess, onTimeout, onRaiseError);	
}
function deleteCommentSuccess(res)
{
    if(res != "false")
    {
        getComments();
    }
}
function previewComment(entryID)
{
    if(!addCommentIsValid())
    {
        commentInfo.innerHTML = g_errors;
    }
    commentInfo.innerHTML = 'Generating preview...';
}
function previewCommentCallback(res)
{
    if(res != "-1")
    {
        commentContainer.innerHTML = res;
        commentInfo.innerHTML = 'If you are happy with your comment, click "Add Comment" to add it permenantly.';
        btnCommentAdd.style.display = 'block';        
    }
}
